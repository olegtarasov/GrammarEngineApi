using System.IO;

namespace GrammarEngineApi
{
    /// <summary>
    /// A token that holds a source word and has a pointer to a grammar entry.
    /// </summary>
    public class EntryToken : IToken
    {
        private string _lemmatized = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        public EntryToken(Entry entry, string sourceWord)
        {
            Entry = entry;
            SourceWord = sourceWord;
        }

        public string LemmatizedWord => _lemmatized ?? (_lemmatized = (Entry.EntryExists ? Entry.Name : SourceWord).ToLower());
        public string SourceWord { get; }
        public bool IsRecognized => Entry.EntryExists;

        /// <summary>
        /// Grammar entry.
        /// </summary>
        public Entry Entry { get; }

        /// <summary>
        /// Saves the entry to a binary stream.
        /// </summary>
        public void Save(BinaryWriter writer)
        {
            writer.Write(Entry.Id);
            writer.Write(SourceWord);
        }

        /// <summary>
        /// Loads an entry from a binary stream.
        /// </summary>
        public static EntryToken Load(BinaryReader reader, GrammarEngine engine)
        {
            int id = reader.ReadInt32();
            string srcWord = reader.ReadString();

            return new EntryToken(engine.GetEntry(id), srcWord);
        }

        public override string ToString()
        {
            return $"{SourceWord} [src: {Entry.Name}, {Entry.WordClass.ToString()}]";
        }
    }
}