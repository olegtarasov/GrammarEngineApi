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

namespace TextUtil
{
    public class App
    {
        [Verb]
        public void Lemmatize(string path, [DefaultValue(null)] string outPath = null)
        {
            var locker = new object();
            var jobs = new ConcurrentQueue<NluJob>();
            int bufferSize = Environment.ProcessorCount * 32;
            var grammar = new GrammarEngine(ConfigurationManager.AppSettings["GrammarPath"]);
            var lemmatizer = new Lemmatizer(grammar);

            Console.WriteLine($"Got {Environment.ProcessorCount} threads, buffer is {bufferSize}");

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

            Console.WriteLine($"Writing lemmatized to {outPath}");

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(outPath))
            {
                var tracker = new ProgressTracker(stream.Length, 10000);

                Console.WriteLine("Loading initial batch");
                for (int i = 0; i < bufferSize; i++)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    string trimmed = line.Trim().ToLower();
                    if (string.IsNullOrEmpty(trimmed))
                    {
                        i--;
                    }
                    else
                    {
                        jobs.Enqueue(new NluJob(trimmed));
                    }
                }

                while (stream.Position < stream.Length)
                {
                    Parallel.ForEach(jobs, job =>
                    {
                        if (job.Type == NluJobType.SplitSentenses)
                        {
                            if (!string.IsNullOrEmpty(job.Line))
                            {
                                var sentenses = grammar.SplitSentenses(job.Line);
                                if (sentenses.Count > 0)
                                {
                                    jobs.Enqueue(new NluJob(sentenses));
                                }
                            }
                        }
                        else if (job.Type == NluJobType.Lemmatize)
                        {
                            if (job.Sentenses?.Count > 0)
                            {
                                var builder = new StringBuilder();

                                for (int sentenseIdx = 0; sentenseIdx < job.Sentenses.Count; sentenseIdx++)
                                {
                                    string sentense = job.Sentenses[sentenseIdx];
                                    var lemmatized = lemmatizer.LemmatizeSentense(sentense);
                                    if (lemmatized.Length > 0)
                                    {
                                        for (int tokenIdx = 0; tokenIdx < lemmatized.Length; tokenIdx++)
                                        {
                                            var item = lemmatized[tokenIdx];
                                            string word = item.IsLemmatized ? item.Word : "UNKNOWN";
                                            builder.Append(word).Append(' ');
                                        }

                                        builder.Append('.');
                                    }
                                }

                                writer.WriteLine(builder.ToString());
                                writer.Flush();
                            }
                        }

                        if (jobs.Count < bufferSize / 2)
                        {
                            while (jobs.Count < bufferSize)
                            {
                                string line;
                                lock (locker)
                                {
                                    line = reader.ReadLine();
                                }

                                if (line == null)
                                {
                                    break;
                                }

                                string trimmed = line.Trim().ToLower();
                                if (!string.IsNullOrEmpty(trimmed))
                                {
                                    jobs.Enqueue(new NluJob(trimmed));
                                }
                            }
                        }

                        if (tracker.ShouldReport(stream.Position))
                        {
                            Console.WriteLine($"Read {((double)stream.Position / stream.Length * 100.0d):F4} %");
                        }
                    });
                }
            }
        }

        private enum NluJobType
        {
            SplitSentenses,
            Lemmatize
        }

        private struct NluJob
        {
            public NluJob(string line) : this()
            {
                Type = NluJobType.SplitSentenses;
                Line = line;
            }

            public NluJob(List<string> sentenses) : this()
            {
                Type = NluJobType.Lemmatize;
                Sentenses = sentenses;
            }

            public NluJobType Type;
            public string Line;
            public List<string> Sentenses;
        }
    }
}
