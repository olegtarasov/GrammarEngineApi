using System;
using System.Linq;
using GrammarEngineApi;


namespace GrammarTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var eng = new GrammarEngine(@"C:\Projects\CognitiveFramework\embeddings\grammar\dictionary.xml");
            var lemmatizer = new Lemmatizer(eng);
            var sentenses = eng.SplitSentenses("Я взял на дачу пятьсот тридцать пять кило кокса, штуку баксов, 10 обезьянок и впупырдцать уздпячников. Превед медвед!");
            var lemmatized = lemmatizer.LemmatizeSentense(sentenses[0]);
            int cnt = GrammarApi.sol_CountEntries(eng.GetEngineHandle());
        }
    }
}
