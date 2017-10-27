using System;

namespace GrammarEngineApi
{
    public class TextSegmenter : IDisposable
    {
        private readonly GrammarEngine _gren;
        private IntPtr _hObject;

        public TextSegmenter(GrammarEngine gren, IntPtr hObject)
        {
            _gren = gren;
            _hObject = hObject;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public string ReadSentence()
        {
            if (GrammarApi.sol_FetchSentence(_hObject) >= 0)
            {
                return GrammarApi.sol_GetFetchedSentenceFX(_hObject);
            }
            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_hObject != IntPtr.Zero)
            {
                GrammarApi.sol_DeleteSentenceBroker(_hObject);
                _hObject = IntPtr.Zero;
            }
        }
    }
}