namespace GrammarEngineApi
{
    /// <summary>
    /// Simple token which is not backed by Grammar Engine.
    /// </summary>
    public class SimpleToken : IToken
    {
        public SimpleToken(string word, bool isRecognized)
        {
            SourceWord = LemmatizedWord = word;
            IsRecognized = isRecognized;
        }

        /// <inheritdoc />
        public string LemmatizedWord { get; }

        /// <inheritdoc />
        public string SourceWord { get; }

        /// <inheritdoc />
        public bool IsRecognized { get; }

        public override string ToString()
        {
            return LemmatizedWord;
        }
    }
}