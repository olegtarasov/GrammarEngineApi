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
            var sentenses = eng.SplitSentenses("Я взял на дачу пятьст тридцать пять кило кокса, штуку баксов, 10 обезьянок. Превед медвед!");
            var lemmatized = lemmatizer.LemmatizeSentense(sentenses[0], MorphologyFlags.SOL_GREN_ALLOW_FUZZY | MorphologyFlags.SOL_GREN_REORDER_TREE);
            int cnt = GrammarApi.sol_CountEntries(eng.GetEngineHandle());
        }
    }
}
