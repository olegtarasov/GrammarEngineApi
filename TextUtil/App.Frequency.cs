using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Linq;
using CLAP;
using GrammarEngineApi;
using GrammarEngineApi.Processors;

namespace TextUtil
{
    public partial class App
    {
        private class Frequency
        {
            public readonly ConcurrentDictionary<string, long> KnownWords = new ConcurrentDictionary<string, long>();
            public readonly ConcurrentDictionary<string, long> UnknownWords = new ConcurrentDictionary<string, long>();
        }

        [Verb]
        public void WordFrequency(string path)
        {
            var enginePool = new GrammarEnginePool(ConfigurationManager.AppSettings["GrammarPath"]);
            var processor = new FileProcessor<Frequency, object>(
                path, 
                enginePool, 
                32,
                fileContextFactory: file => new Frequency(),
                sentenceContextFactory: fileContext => null,
                action: (job, frequency) =>
                {
                    if (string.IsNullOrEmpty(job.Sentence))
                    {
                        return;
                    }

                    var engine = enginePool.GetInstance();
                    var tokens = engine.TokenizeSentence(job.Sentence, Languages.RUSSIAN_LANGUAGE);
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        string token = tokens[i];
                        using (var forms = engine.FindWordForms(tokens[i]))
                        {
                            if (forms.Count == 0)
                            {
                                frequency.UnknownWords.AddOrUpdate(token.ToLower(), 1, (s, cur) => cur + 1);
                                continue;
                            }

                            var entry = new Entry(engine, forms.GetEntryKey(0));
                            if (entry.WordClass == WordClassesRu.PUNCTUATION_class
                                || entry.WordClass == WordClassesRu.NUMBER_CLASS_ru
                                || entry.WordClass == WordClassesRu.NUM_WORD_CLASS)
                            {
                                continue;
                            }

                            frequency.KnownWords.AddOrUpdate(token.ToLower(), 1, (s, cur) => cur + 1);
                        }
                    }

                    enginePool.ReturnInstance(engine);
                },
                fileFinalizer: (file, frequency) =>
                {
                    string dir = Path.Combine(Path.GetDirectoryName(file), "frequency");
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string ext = Path.GetExtension(file);
                    string knownPath = Path.Combine(dir, fileName + "_known" + ext);
                    string unknownPath = Path.Combine(dir, fileName + "_unknown" + ext);

                    using (var writer = new StreamWriter(knownPath))
                    {
                        foreach (var pair in frequency.KnownWords.OrderByDescending(x => x.Value))
                        {
                            writer.WriteLine($"{pair.Key};{pair.Value}");
                        }
                    }

                    using (var writer = new StreamWriter(unknownPath))
                    {
                        foreach (var pair in frequency.UnknownWords.OrderByDescending(x => x.Value))
                        {
                            writer.WriteLine($"{pair.Key};{pair.Value}");
                        }
                    }
                });

            processor.Process();
        }

        [Verb]
        public void AggregateFrequency(string dir)
        {
            if (!Directory.Exists(dir))
            {
                throw new InvalidOperationException("Directory doesn't exist!");
            }

            var knownFiles = Directory.GetFiles(dir, "*_known.txt");
            var unknownFiles = Directory.GetFiles(dir, "*_unknown.txt");

            if (knownFiles.Length == 0 && unknownFiles.Length == 0)
            {
                throw new InvalidOperationException("No files to process!");
            }

            var known = AggregateFiles(knownFiles);
            var unknown = AggregateFiles(unknownFiles);

            using (var writer = new StreamWriter(Path.Combine(dir, "kwnown_total.txt")))
            {
                foreach (var pair in known.OrderByDescending(x => x.Value))
                {
                    writer.WriteLine($"{pair.Key};{pair.Value}");
                }
            }

            using (var writer = new StreamWriter(Path.Combine(dir, "unkwnown_total.txt")))
            {
                foreach (var pair in unknown.OrderByDescending(x => x.Value))
                {
                    writer.WriteLine($"{pair.Key};{pair.Value}");
                }
            }
        }

        private ConcurrentDictionary<string, long> AggregateFiles(string[] files)
        {
            var result = new ConcurrentDictionary<string, long>();

            foreach (var file in files)
            {
                _log.Info($"Processing file {file}");

                using (var reader = new StreamReader(file))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        int pos = line.LastIndexOf(';');
                        if (pos == -1)
                        {
                            continue;
                        }

                        string word = line.Substring(0, pos);
                        if (!string.IsNullOrEmpty(word)
                            && long.TryParse(line.Substring(pos + 1), out var num))
                        {
                            result.AddOrUpdate(word, num, (s, existing) => existing + num);
                        }
                    }
                }
            }

            return result;
        }
    }
}