using System;

namespace GrammarEngineApi
{
    /// <summary>
    /// A set of flags affecting lemmatization.
    /// </summary>
    [Flags]
    public enum LemmatizerFlags
    {
        /// <summary>
        /// Default mode.
        /// </summary>
        Default = 0x00000000,

        /// <summary>
        /// Faster mode.
        /// </summary>
        Faster = 0x00000001,

        /// <summary>
        /// Fastest mode.
        /// </summary>
        Fastest = 0x00000002
    }
}