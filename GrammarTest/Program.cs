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
            var morph = eng.AnalyzeMorphology("Я взял на дачу пятьсот тридцать пять кило кокса, штуку баксов и 10 обезьянок.", Languages.RUSSIAN_LANGUAGE, MorphologyFlags.SOL_GREN_MODEL | MorphologyFlags.SOL_GREN_MODEL_ONLY);

            var entries = morph.Nodes.Skip(1).Take(morph.Nodes.Count - 2).Select(x => x.Entry).ToList();

            var entry = morph.Nodes[5].Entry;
            var pairs = morph.Nodes[5].Pairs;
            var codes = pairs.Select(x => eng.GetCoordName(x.CoordId)).ToList();
        }
    }
}
