namespace GrammarEngineApi
{
    public class LemmatizedToken
    {
        public LemmatizedToken(SyntaxTreeNode sourceNode, Entry lemmatizedEntry)
        {
            SourceNode = sourceNode;
            LemmatizedEntry = lemmatizedEntry;
            Word = lemmatizedEntry.Word.ToLower();
        }

        public SyntaxTreeNode SourceNode { get; }
        public Entry LemmatizedEntry { get;  }
        public string Word { get; }

        public override string ToString()
        {
            return $"{Word} [src: {SourceNode.Entry}]";
        }
    }
}