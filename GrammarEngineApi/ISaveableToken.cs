using System.IO;

namespace GrammarEngineApi
{
    /// <summary>
    /// A token that can be saved to a binary stream.
    /// </summary>
    public interface ISaveableToken : IToken
    {
        /// <summary>
        /// Saves the entry to a binary stream.
        /// </summary>
        void Save(BinaryWriter writer);
    }
}