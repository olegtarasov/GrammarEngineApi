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
    public partial class App
    {
        private ILog _log = LogManager.GetLogger(typeof(App));

        [Verb]
        public void Test()
        {
            var engine = new GrammarEngine(/*"/home/oleg/host/Projects/GrammarEngine/bin/linux/dictionary/x64/dictionary.xml"*/);
        }
        
        
        //private static void LemmatizeFastJob(NluJob job, GrammarEnginePool enginePool, StreamWriter writer)
        //{
        //    if (job.Sentences?.Count == 0)
        //    {
        //        return;
        //    }

        //    var builder = new StringBuilder();
            
        //    var engine = enginePool.GetInstance();

        //    for (int i = 0; i < job.Sentences.Count; i++)
        //    {
        //        string sentence = job.Sentences[i];
        //        string tokens = engine.TokenizeWithSeparator(sentence, Languages.RUSSIAN_LANGUAGE);
        //        var lemmatized = engine.LemmatizeSentence(tokens);
                
        //        for (int lemmaIdx = 0; lemmaIdx < lemmatized.Length; lemmaIdx++)
        //        {
        //            builder.Append(lemmatized[lemmaIdx].ToLower()).Append(' ');
        //        }
        //    }

        //    enginePool.ReturnInstance(engine);

        //    if (builder.Length > 0)
        //    {
        //        writer.WriteLine(builder.ToString());
        //        writer.Flush();
        //    }
        //}
    }
}
