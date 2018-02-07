using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            public Token[] Tokens = null;
            public bool IsReady = false;
        }

        private class MorphologyFileContext
        {
            private readonly ManualResetEvent _trigger = new ManualResetEvent(false);
            private readonly ConcurrentQueue<Sentence> _sentences = new ConcurrentQueue<Sentence>();
            private readonly Task _writerTask;
            private readonly FileStream _stream;
            private readonly BinaryWriter _writer;

            private bool _fileComplete = false;

            public MorphologyFileContext(string sourceFile)
            {
                string dir = Path.Combine(Path.GetDirectoryName(sourceFile), "morphology");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                string outPath = Path.Combine(dir, Path.GetFileNameWithoutExtension(sourceFile) + ".bm");
                _stream = new FileStream(outPath, FileMode.Create, FileAccess.Write);
                _writer = new BinaryWriter(_stream);

                _writerTask = new Task(() =>
                {
                    while (!_fileComplete)
                    {
                        _trigger.Reset();
                        _trigger.WaitOne();

                        while (_sentences.TryPeek(out var sentence) && sentence.IsReady)
                        {
                            if (!_sentences.TryDequeue(out sentence) || !sentence.IsReady)
                            {
                                throw new InvalidOperationException("Unsynchronized read from the queue!");
                            }

                            if (sentence.Tokens == null || sentence.Tokens.Length == 0)
                            {
                                continue;
                            }

                            _writer.Write(sentence.Tokens.Length);
                            for (int i = 0; i < sentence.Tokens.Length; i++)
                            {
                                var token = sentence.Tokens[i];
                                _writer.Write(token.EntryId);
                                _writer.Write(token.Source);
                            }
                        }
                    }

                    _writer.Dispose();
                    _stream.Dispose();
                });

                _writerTask.Start();
            }

            public Sentence CreateSentence()
            {
                var result = new Sentence();
                _sentences.Enqueue(result);
                return result;
            }

            public void Flush()
            {
                _trigger.Set();
            }

            public void FinalizeFile()
            {
                _fileComplete = true;
                _trigger.Set();
            }
        }

        [Verb]
        public void MorphologyTest(string path)
        {
            var engine = new GrammarEngine(ConfigurationManager.AppSettings["GrammarPath"]);

            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed))
                    {
                        continue;
                    }

                    long cnt = 0;
                    var sentences = engine.SplitSentences(trimmed);

                    foreach (var sentence in sentences)
                    {
                        using (var morph = engine.AnalyzeMorphology(sentence, Languages.RUSSIAN_LANGUAGE, MorphologyFlags.SOL_GREN_MODEL | MorphologyFlags.SOL_GREN_MODEL_ONLY))
                        {
                            for (int i = 0; i < morph.Nodes.Length; i++)
                            {
                                cnt++;
                            }
                        }
                    }
                }
            }
        }

        [Verb]
        public void Morphology(string path, [DefaultValue(false)] bool sequential)
        {
            var enginePool = new GrammarEnginePool(ConfigurationManager.AppSettings["GrammarPath"]);
            const int batchSize = 16;

            _log.Info($"Got {Environment.ProcessorCount} threads. Batch size for each thread: {batchSize}.");
            
            var processor = new FileProcessor<MorphologyFileContext, Sentence>(
                path, 
                enginePool, 
                batchSize,
                fileContextFactory: file => new MorphologyFileContext(file),
                sentenceContextFactory: fileContext => fileContext.CreateSentence(), 
                action: (job, context) =>
                {
                    if (string.IsNullOrEmpty(job.Sentence))
                    {
                        job.Context.IsReady = true;
                        context.Flush();
                        return;
                    }

                    var engine = enginePool.GetInstance();
                    using (var lemmatized = engine.AnalyzeMorphology(job.Sentence, Languages.RUSSIAN_LANGUAGE, MorphologyFlags.SOL_GREN_MODEL | MorphologyFlags.SOL_GREN_MODEL_ONLY))
                    {
                        if (lemmatized.Nodes.Length == 0)
                        {
                            return;
                        }

                        var tokens = new Token[lemmatized.Nodes.Length];
                        for (int i = 0; i < lemmatized.Nodes.Length; i++)
                        {
                            var node = lemmatized.Nodes[i];
                            tokens[i] = new Token(node.SourceWord, node.Entry.Id);
                        }

                        job.Context.Tokens = tokens;
                        job.Context.IsReady = true;
                    }

                    enginePool.ReturnInstance(engine);
                    context.Flush();
                },
                fileFinalizer: (file, context) =>
                {
                    context.FinalizeFile();
                }, 
                numTasks: sequential ? 1 : Environment.ProcessorCount);

            processor.Process();
        }
    }
}