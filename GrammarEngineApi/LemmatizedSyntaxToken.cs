namespace GrammarEngineApi
{
    /// <summary>
    /// GrammarEngine-backed version of lemmatized token.
    /// </summary>
    public class LemmatizedSyntaxToken : ILemmatizedToken
    {
        internal LemmatizedSyntaxToken(SyntaxTreeNode syntaxNode)
        {
            SyntaxNode = syntaxNode;
            IsLemmatized = syntaxNode.GrammarEntry.EntryExists;
            Word = IsLemmatized ? syntaxNode.GrammarEntry.Word.ToLower() : syntaxNode.SourceWord;
        }

        /// <summary>
        /// Syntax node created in a process of lemmatization.
        /// </summary>
        public SyntaxTreeNode SyntaxNode { get; }


        /// <inheritdoc />
        public string Word { get; }

        /// <inheritdoc />
        public bool IsLemmatized { get; }

        public override string ToString()
        {
            return $"{Word} [src: {SyntaxNode.SourceWord}]";
        }
    }
}