namespace GrammarEngineApi
{
    /// <summary>
    /// Represents a lemmatized token.
    /// </summary>
    public interface IToken
    {
        /// <summary>
        /// Gets lemmatized form of a word if <see cref="IsRecognized"/> is <code>true</code> or
        /// source word is it's <code>false</code>.
        /// </summary>
        string LemmatizedWord { get; }

        /// <summary>
        /// Always returns source word.
        /// </summary>
        string SourceWord { get; }

        /// <summary>
        /// Indicates whether source word was lemmatized.
        /// </summary>
        bool IsRecognized { get; }
    }
}