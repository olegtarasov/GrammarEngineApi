namespace GrammarEngineApi
{
    public class LemmatizedToken
    {
        public LemmatizedToken(SyntaxTreeNode sourceNode, Entry lemmatizedEntry)
        {
            SourceNode = sourceNode;
            LemmatizedEntry = lemmatizedEntry;
            Word = lemmatizedEntry.EntryExists ? lemmatizedEntry.Word.ToLower() : sourceNode.Word;
            Lemmatized = lemmatizedEntry.EntryExists;
        }

        public SyntaxTreeNode SourceNode { get; }
        public Entry LemmatizedEntry { get;  }
        public string Word { get; }
        public bool Lemmatized { get; }

        public override string ToString()
        {
            return $"{Word} [src: {SourceNode.Entry}]";
        }
    }
}