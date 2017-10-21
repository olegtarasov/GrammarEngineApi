using System;
using System.Linq;
using GrammarEngineApi;

namespace GrammarTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var eng = new GrammarEngine(@"D:\Downloads\parser-ru-win64\dictionary.xml");
            var morph = eng.AnalyzeMorphology("Я взял на дачу пятьсот тридцать пять кило кокса.", Languages.RUSSIAN_LANGUAGE, MorphologyFlags.SOL_GREN_MODEL | MorphologyFlags.SOL_GREN_MODEL_ONLY);

            var id = morph.Nodes[5].GetEntryID();
            int c = eng.GetEntryClass(id);
            var pairs = morph.Nodes[5].GetPairs();
            var codes = pairs.Select(x => eng.GetCoordName(x.CoordId)).ToList();
        }
    }
}
