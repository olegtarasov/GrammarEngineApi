using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrammarEngineApi;

namespace GrammarTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var eng = new GrammarEngine(@"C:\Projects\CognitiveFramework\embeddings\grammar\dictionary.xml");
            var lemmatizer = new Lemmatizer(eng);
            var sentenses = eng.SplitSentenses("Я взял на дачу пятьсот тридцать пять кило кокса, штуку баксов, 10 обезьянок. Превед медвед!");
            var tokens = eng.Tokenize(sentenses[0], Languages.RUSSIAN_LANGUAGE);
            string agg = tokens.Aggregate((s, s1) => s + ' ' + s1);
            var lemmatized = lemmatizer.LemmatizeSentense(agg);
            int cnt = GrammarApi.sol_CountEntries(eng.GetEngineHandle());
        }
    }
}
