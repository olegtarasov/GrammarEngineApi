using System.IO;

namespace GrammarEngineApi
{
    /// <summary>
    /// A token that has an associated grammar entry and can be saved to a stream.
    /// </summary>
    public interface IEntryToken : IToken
    {
        /// <summary>
        /// Grammar entry for the token.
        /// </summary>
        Entry Entry { get; }

        /// <summary>
        /// Saves the entry to a binary stream.
        /// </summary>
        void Save(BinaryWriter writer);
    }
}