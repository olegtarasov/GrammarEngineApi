using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using CLAP;
using GrammarEngineApi;

namespace TextUtil
{
    public partial class App
    {
        [Verb]
        public void Lemmatize(string path, [DefaultValue(null)] string outPath = null)
        {
            var enginePool = new GrammarEnginePool(ConfigurationManager.AppSettings["GrammarPath"]);
            const int batchSize = 16;

            _log.Info($"Got {Environment.ProcessorCount} threads. Batch size for each thread: {batchSize}.");
            _log.Info($"Lemmatization mode: {(false ? "FAST" : "ACCURATE")}.");

            if (string.IsNullOrEmpty(outPath))
            {
                outPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + "_lemmatized" + Path.GetExtension(path));
            }
            else
            {
                string dir = Path.GetDirectoryName(outPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            _log.Info($"Writing lemmatized to {outPath}");

            using (var writer = new StreamWriter(outPath))
            {
                var eng = enginePool.GetInstance();
                var segmenter = eng.CreateTextFileSegmenter(path, true, Languages.RUSSIAN_LANGUAGE);
                enginePool.ReturnInstance(eng);
                var processor = new ParallelSentenceProcessor(
                    segmenter, batchSize,
                    action: sentence =>
                    {
                        var builder = new StringBuilder();
                        var engine = enginePool.GetInstance();
                        using (var lemmatized = engine.AnalyzeMorphology(sentence, Languages.RUSSIAN_LANGUAGE, MorphologyFlags.SOL_GREN_MODEL | MorphologyFlags.SOL_GREN_MODEL_ONLY))
                        {
                            if (lemmatized.Nodes.Length > 0)
                            {
                                for (int tokenIdx = 0; tokenIdx < lemmatized.Nodes.Length; tokenIdx++)
                                {
                                    var item = lemmatized.Nodes[tokenIdx];
                                    string word = null;
                                    if (item.GrammarEntry.EntryExists)
                                    {
                                        switch (item.GrammarEntry.WordClass)
                                        {
                                            case WordClassesRu.NUMBER_CLASS_ru:
                                            case WordClassesRu.NUM_WORD_CLASS:
                                                word = "NUM";
                                                break;
                                            case WordClassesRu.PUNCTUATION_class:
                                                break;
                                            case WordClassesRu.UNKNOWN_ENTRIES_CLASS:
                                                word = "UNK";
                                                break;
                                            default:
                                                word = item.Word;
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        word = "UNK";
                                    }

                                    if (word != null)
                                    {
                                        builder.Append(word).Append(' ');
                                    }
                                }
                            }
                        }

                        enginePool.ReturnInstance(engine);

                        if (builder.Length > 0)
                        {
                            writer.WriteLine(builder.ToString());
                            writer.Flush();
                        }
                    }/*, numTasks: 1*/);

                processor.Process();
            }
        }
    }
}