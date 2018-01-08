using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Linq;
using CLAP;
using GrammarEngineApi;

namespace TextUtil
{
    public partial class App
    {
        [Verb]
        public void WordFrequency(string path)
        {
            var enginePool = new GrammarEnginePool(ConfigurationManager.AppSettings["GrammarPath"]);
            var eng = enginePool.GetInstance();
            var segmenter = eng.CreateTextFileSegmenter(path, true, Languages.RUSSIAN_LANGUAGE);
            enginePool.ReturnInstance(eng);

            var known = new ConcurrentDictionary<string, int>();
            var unknown = new ConcurrentDictionary<string, int>();
            var processor = new ParallelSentenceProcessor(segmenter, 32,
                sentence =>
                {
                    if (string.IsNullOrEmpty(sentence))
                    {
                        return;
                    }

                    var engine = enginePool.GetInstance();
                    var tokens = engine.TokenizeSentence(sentence, Languages.RUSSIAN_LANGUAGE);
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        string token = tokens[i];
                        using (var forms = engine.FindWordForms(tokens[i]))
                        {
                            if (forms.Count == 0)
                            {
                                unknown.AddOrUpdate(token.ToLower(), 1, (s, cur) => cur + 1);
                                continue;
                            }

                            var entry = new Entry(engine, forms.GetEntryKey(0));
                            if (entry.WordClass == WordClassesRu.PUNCTUATION_class
                                || entry.WordClass == WordClassesRu.NUMBER_CLASS_ru
                                || entry.WordClass == WordClassesRu.NUM_WORD_CLASS)
                            {
                                continue;
                            }

                            known.AddOrUpdate(token.ToLower(), 1, (s, cur) => cur + 1);
                        }
                    }

                    enginePool.ReturnInstance(engine);
                });

            processor.Process();

            string dir = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            string knownPath = Path.Combine(dir, fileName + "_known" + ext);
            string unknownPath = Path.Combine(dir, fileName + "_unknown" + ext);

            using (var writer = new StreamWriter(knownPath))
            {
                foreach (var pair in known.OrderByDescending(x => x.Value))
                {
                    writer.WriteLine($"{pair.Key};{pair.Value}");
                }
            }

            using (var writer = new StreamWriter(unknownPath))
            {
                foreach (var pair in unknown.OrderByDescending(x => x.Value))
                {
                    writer.WriteLine($"{pair.Key};{pair.Value}");
                }
            }
        }
    }
}