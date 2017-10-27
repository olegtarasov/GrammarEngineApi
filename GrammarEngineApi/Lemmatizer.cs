namespace GrammarEngineApi
{
    public class Lemmatizer
    {
        private readonly GrammarEngine _engine;

        public Lemmatizer(GrammarEngine engine)
        {
            _engine = engine;
        }

        public LemmatizedToken[] LemmatizeSentense(string sentense)
        {
            var res = _engine.AnalyzeMorphology(sentense, Languages.RUSSIAN_LANGUAGE, MorphologyFlags.SOL_GREN_MODEL | MorphologyFlags.SOL_GREN_MODEL_ONLY);
            if (res == null || res.Nodes.Count <= 2)
            {
                return new LemmatizedToken[0];
            }

            var result = new LemmatizedToken[res.Nodes.Count - 2];
            for (int i = 1; i < res.Nodes.Count - 1; i++)
            {
                var node = res.Nodes[i];
                result[i - 1] = new LemmatizedToken(node, node.Entry);
            }

            return result;
        }
    }
}