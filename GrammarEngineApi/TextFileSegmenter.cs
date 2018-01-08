using System;
using System.Text;
using System.Threading;
using GrammarEngineApi.Api;

namespace GrammarEngineApi
{
    /// <summary>
    /// Segmenter which splits text file into sentences. No thread synchronization
    /// is performed.
    /// </summary>
    public class TextFileSegmenter : IDisposable
    {
        private readonly IntPtr _hObject;

        private bool _disposed = false;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="hObject">Broker object.</param>
        internal TextFileSegmenter(IntPtr hObject)
        {
            _hObject = hObject;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Reads the next sentence.
        /// </summary>
        /// <returns>Sentence or null if reached the end of file.</returns>
        public string ReadSentence()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Segmenter disposed.");
            }

            int len;
            if ((len = GrammarApi.sol_FetchSentence(_hObject)) < 0)
            {
                CanRead = false;
                return null;
            }

            if (len == 0)
            {
                return string.Empty;
            }

            var b = new StringBuilder(len + 2);
            GrammarApi.sol_GetFetchedSentence(_hObject, b);
            return b.ToString();
        }

        /// <summary>
        /// Indicates whether last read fetched a sentence so the next read
        /// might too.
        /// </summary>
        public bool CanRead { get; private set; } = true;

        /// <summary>
        /// Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                Interlocked.MemoryBarrier();
                GrammarApi.sol_DeleteSentenceBroker(_hObject);
            }
        }
    }
}