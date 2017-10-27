using System;
using System.Linq;
using GrammarEngineApi;


namespace GrammarTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var eng = new GrammarEngine(@"C:\Users\ovtaras4\Downloads\parser-ru-win64\dictionary.xml");
            var lemmatizer = new Lemmatizer(eng);
            var lemmatized = lemmatizer.LemmatizeSentense("Я взял на дачу пятьсот тридцать пять кило кокса, штуку баксов, 10 обезьянок и впупырдцать уздпячников.");
            int cnt = GrammarApi.sol_CountEntries(eng.GetEngineHandle());
        }
    }
}
