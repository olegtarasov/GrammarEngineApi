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

        public WordClassesRu WordClass => (WordClassesRu)_engine.GetEntryClass(_entryId);
    }
}