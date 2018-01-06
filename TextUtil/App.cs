using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLAP;
using GrammarEngineApi;
using log4net;

// Lemmatize -path="C:\_Models\all.1.plain.txt"

namespace TextUtil
{
    public class App
    {
        private ILog _log = LogManager.GetLogger(typeof(App));

        [Verb]
        public void TestLemmatizer()
        {
            var engine = new GrammarEngine(ConfigurationManager.AppSettings["GrammarPath"]);
            var result = engine.LemmatizeSentense("Мы|ели|суп|а|ели|шумели");
        }

        [Verb]
        public void Lemmatize(string path, [DefaultValue(false)] bool fast = false, [DefaultValue(null)] string outPath = null)
        {
            var locker = new object();
            var jobs = new ConcurrentQueue<NluJob>();
            var enginePool = new GrammarEnginePool(ConfigurationManager.AppSettings["GrammarPath"]);
            const int batchSize = 16;

            _log.Info($"Got {Environment.ProcessorCount} threads. Batch size for each thread: {batchSize}.");
            _log.Info($"Lemmatization mode: {(fast ? "FAST" : "ACCURATE")}.");

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

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(outPath))
            {
                var tracker = new ProgressTracker(stream.Length, 10000);
                var processor = new ParallelProcessor<NluJob>(
                    jobs, batchSize, 
                    action: job =>
                    {
                        if (job.Type == NluJobType.SplitSentenses)
                        {
                            if (!string.IsNullOrEmpty(job.Line))
                            {
                                var engine = enginePool.GetInstance();
                                var sentenses = engine.SplitSentenses(job.Line);
                                enginePool.ReturnInstance(engine);
                                if (sentenses.Count > 0)
                                {
                                    jobs.Enqueue(new NluJob(sentenses, fast));
                                }
                            }
                        }
                        else if (job.Type == NluJobType.Lemmatize)
                        {
                            LemmatizeJob(job, enginePool, writer);
                        }
                        else if (job.Type == NluJobType.LemmatizeFast)
                        {
                            LemmatizeFastJob(job, enginePool, writer);
                        }
                    },
                    producer: () =>
                    {
                        string line;
                        lock (locker)
                        {
                            line = reader.ReadLine();
                        }

                        if (tracker.ShouldReport(stream.Position))
                        {
                            _log.Info($"Read {((double)stream.Position / stream.Length * 100.0d):F4} %");
                        }

                        if (line == null)
                        {
                            return new NluJob();
                        }

                        return new NluJob(line.Trim());
                    },
                    canProduce: () => stream.Position < stream.Length/*, numTasks: 1*/);

                processor.Process();
            }
        }

        private static void LemmatizeFastJob(NluJob job, GrammarEnginePool enginePool, StreamWriter writer)
        {
            if (job.Sentenses?.Count == 0)
            {
                return;
            }

            var builder = new StringBuilder();
            
            var engine = enginePool.GetInstance();

            for (int i = 0; i < job.Sentenses.Count; i++)
            {
                string sentense = job.Sentenses[i];
                string tokens = engine.TokenizeWithSeparator(sentense, Languages.RUSSIAN_LANGUAGE);
                var lemmatized = engine.LemmatizeSentense(tokens);
                
                for (int lemmaIdx = 0; lemmaIdx < lemmatized.Length; lemmaIdx++)
                {
                    builder.Append(lemmatized[lemmaIdx].ToLower()).Append(' ');
                }
            }

            enginePool.ReturnInstance(engine);

            if (builder.Length > 0)
            {
                writer.WriteLine(builder.ToString());
                writer.Flush();
            }
        }

        private static void LemmatizeJob(NluJob job, GrammarEnginePool enginePool, StreamWriter writer)
        {
            if (job.Sentenses?.Count == 0)
            {
                return;
            }

            var builder = new StringBuilder();

            for (int sentenseIdx = 0; sentenseIdx < job.Sentenses.Count; sentenseIdx++)
            {
                string sentense = job.Sentenses[sentenseIdx];
                var engine = enginePool.GetInstance();
                using (var lemmatized = engine.AnalyzeMorphology(sentense, Languages.RUSSIAN_LANGUAGE, MorphologyFlags.SOL_GREN_MODEL | MorphologyFlags.SOL_GREN_MODEL_ONLY))
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
            }

            if (builder.Length > 0)
            {
                writer.WriteLine(builder.ToString());
                writer.Flush();
            }
        }

        private enum NluJobType
        {
            None,
            SplitSentenses,
            Lemmatize,
            LemmatizeFast
        }

        private struct NluJob
        {
            public NluJob(string line) : this()
            {
                Type = NluJobType.SplitSentenses;
                Line = line;
            }

            public NluJob(List<string> sentenses, bool fast) : this()
            {
                Type = fast ? NluJobType.LemmatizeFast : NluJobType.Lemmatize;
                Sentenses = sentenses;
            }

            public NluJobType Type;
            public string Line;
            public List<string> Sentenses;

            public override string ToString()
            {
                return $"{Type}";
            }
        }
    }
}
