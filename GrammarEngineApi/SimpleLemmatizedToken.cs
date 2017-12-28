namespace GrammarEngineApi
{
    /// <summary>
    /// Simple token which is not backed by Grammar Engine.
    /// </summary>
    public class SimpleLemmatizedToken : ILemmatizedToken
    {
        public SimpleLemmatizedToken(string word, bool isLemmatized)
        {
            Word = word;
            IsLemmatized = isLemmatized;
        }

        /// <inheritdoc />
        public string Word { get; }

        /// <inheritdoc />
        public bool IsLemmatized { get; }
    }
}