namespace GrammarEngineApi
{
    /// <summary>
    /// Represents a lemmatized token.
    /// </summary>
    public interface ILemmatizedToken
    {
        /// <summary>
        /// Gets lemmatized form of a word if <see cref="IsLemmatized"/> is <code>true</code> or
        /// source word is it's <code>false</code>.
        /// </summary>
        string Word { get; }

        /// <summary>
        /// Indicates whether source word was lemmatized.
        /// </summary>
        bool IsLemmatized { get; }
    }
}