namespace GrammarEngineApi
{
    /// <summary>
    ///     Grammar entry for a word.
    /// </summary>
    public class Entry
    {
        private readonly GrammarEngine _engine;
        private readonly int _entryId;

        private string _word;

        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="engine">Grammar engine instance.</param>
        /// <param name="entryId">Entry id.</param>
        public Entry(GrammarEngine engine, int entryId)
        {
            _engine = engine;
            _entryId = entryId;
        }

        /// <summary>
        /// Indicates whether an entry exists.
        /// </summary>
        public bool EntryExists => WordClass != WordClassesRu.UNKNOWN_ENTRIES_CLASS && Name != "???";
        
        /// <summary>
        /// Entry name, which is usually a canonical form of the word.
        /// </summary>
        public string Name => _word ?? (_word = _engine.GetEntryName(_entryId));

        /// <summary>
        /// Word class.
        /// </summary>
        public WordClassesRu WordClass => (WordClassesRu)_engine.GetEntryClass(_entryId);

        public override string ToString()
        {
            return $"{Name} [{WordClass}]";
        }
    }
}