namespace GrammarEngineApi
{
    public struct EntryToken
    {
        public EntryToken(Entry entry, string sourceWord)
        {
            Entry = entry;
            SourceWord = sourceWord;
        }

        public string SourceWord { get; }
        public Entry Entry { get; }

        public override string ToString()
        {
            return $"{SourceWord} [src: {Entry.Name}, {Entry.WordClass.ToString()}]";
        }
    }
}