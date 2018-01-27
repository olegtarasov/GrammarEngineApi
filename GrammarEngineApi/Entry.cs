namespace GrammarEngineApi
{
    /// <summary>
    ///     Grammar entry for a word.
    /// </summary>
    public struct Entry
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="entryId">Entry id.</param>
        /// <param name="name">Entry name.</param>
        /// <param name="wordClass">Word class.</param>
        public Entry(int entryId, string name, WordClassesRu wordClass)
        {
            Id = entryId;
            WordClass = wordClass;
            Name = name;
        }

        /// <summary>
        /// Entry id.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Indicates whether an entry exists.
        /// </summary>
        public bool EntryExists => WordClass != WordClassesRu.UNKNOWN_ENTRIES_CLASS && Name != "???";
        
        /// <summary>
        /// Entry name, which is usually a canonical form of the word.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Word class.
        /// </summary>
        public WordClassesRu WordClass { get; }

        public override string ToString()
        {
            return $"{Name} [{WordClass}]";
        }
    }
}