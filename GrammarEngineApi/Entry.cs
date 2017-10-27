namespace GrammarEngineApi
{
    public class Entry
    {
        private readonly GrammarEngine _engine;
        private readonly int _entryId;

        public Entry(GrammarEngine engine, int entryId)
        {
            _engine = engine;
            _entryId = entryId;
        }

        private string _word;
        public string Word => _word ?? (_word = _engine.GetEntryName(_entryId));

        public WordClassesRu WordClass => (WordClassesRu)_engine.GetEntryClass(_entryId);

        public bool EntryExists => Word != "???";

        public override string ToString()
        {
            return $"{Word} [{WordClass}]";
        }
    }
}