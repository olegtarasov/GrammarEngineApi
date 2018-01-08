using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using CLAP;
using GrammarEngineApi;
using GrammarEngineApi.Processors;

namespace TextUtil
{
    public partial class App
    {
        private class Token
        {
            public readonly string Source;
            public readonly int EntryId;

            public Token(string source, int entryId)
            {
                Source = source;
                EntryId = entryId;
            }
        }

        private class Sentence
        {
            public readonly Token[] Tokens;

            public Sentence(Token[] tokens)
            {
                Tokens = tokens;
            }
        }

        private class MorphologyContext
        {
            public readonly LinkedList<Sentence> Sentences = new LinkedList<Sentence>();
        }

        [Verb]
        public void Morphology(string path, [DefaultValue(false)] bool sequential)
        {
            var enginePool = new GrammarEnginePool(ConfigurationManager.AppSettings["GrammarPath"]);
            const int batchSize = 16;

            _log.Info($"Got {Environment.ProcessorCount} threads. Batch size for each thread: {batchSize}.");
            
            var processor = new FileProcessor<MorphologyContext>(path, enginePool, batchSize,
                () => new MorphologyContext(),
                (sentence, context) =>
                {
                    if (string.IsNullOrEmpty(sentence))
                    {
                        return;
                    }

                    var engine = enginePool.GetInstance();
                    using (var lemmatized = engine.AnalyzeMorphology(sentence, Languages.RUSSIAN_LANGUAGE, MorphologyFlags.SOL_GREN_MODEL | MorphologyFlags.SOL_GREN_MODEL_ONLY))
                    {
                        if (lemmatized.Nodes.Length == 0)
                        {
                            return;
                        }

                        var tokens = new Token[lemmatized.Nodes.Length];
                        for (int i = 0; i < lemmatized.Nodes.Length; i++)
                        {
                            var node = lemmatized.Nodes[i];
                            tokens[i] = new Token(node.SourceWord, node.GrammarEntry.Id);
                        }

                        context.Sentences.AddLast(new Sentence(tokens));
                    }

                    enginePool.ReturnInstance(engine);
                },
                (file, context) =>
                {
                    string dir = Path.Combine(Path.GetDirectoryName(file), "morphology");
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    string outPath = Path.Combine(dir, Path.GetFileNameWithoutExtension(file) + ".bm");
                    using (var stream = new FileStream(outPath, FileMode.Create, FileAccess.Write))
                    using (var writer = new BinaryWriter(stream))
                    {
                        foreach (var sentence in context.Sentences)
                        {
                            writer.Write(sentence.Tokens.Length);
                            for (int i = 0; i < sentence.Tokens.Length; i++)
                            {
                                var token = sentence.Tokens[i];
                                writer.Write(token.EntryId);
                                writer.Write(token.Source);
                            }
                        }
                    }
                }, sequential ? 1 : Environment.ProcessorCount);

            processor.Process();
        }
    }
}