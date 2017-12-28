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
        }

        /// <summary>
        /// Syntax node created in a process of lemmatization.
        /// </summary>
        public SyntaxTreeNode SyntaxNode { get; }

        /// <inheritdoc />
        public string Word => IsLemmatized ? SyntaxNode.GrammarEntry.Word.ToLower() : SyntaxNode.SourceWord;

        /// <inheritdoc />
        public string SourceWord => SyntaxNode.SourceWord;

        /// <inheritdoc />
        public bool IsLemmatized => SyntaxNode.GrammarEntry.EntryExists;

        public override string ToString()
        {
            return $"{Word} [src: {SyntaxNode.SourceWord}]";
        }
    }
}