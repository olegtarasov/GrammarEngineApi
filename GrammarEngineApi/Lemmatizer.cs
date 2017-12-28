namespace GrammarEngineApi
{
    public class Lemmatizer
    {
        private readonly GrammarEngine _engine;

        public Lemmatizer(GrammarEngine engine)
        {
            _engine = engine;
        }

        public LemmatizedSyntaxToken[] LemmatizeSentense(string sentense, MorphologyFlags flags = MorphologyFlags.SOL_GREN_MODEL | MorphologyFlags.SOL_GREN_MODEL_ONLY)
        {
            var res = _engine.AnalyzeMorphology(sentense, Languages.RUSSIAN_LANGUAGE, flags);
            if (res == null || res.Nodes.Count <= 2)
            {
                return new LemmatizedSyntaxToken[0];
            }

            var result = new LemmatizedSyntaxToken[res.Nodes.Count - 2];
            for (int i = 1; i < res.Nodes.Count - 1; i++)
            {
                var node = res.Nodes[i];
                result[i - 1] = new LemmatizedSyntaxToken(node);
            }

            return result;
        }
    }
}