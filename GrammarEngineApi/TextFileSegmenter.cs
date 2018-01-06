using System;
using System.Text;
using GrammarEngineApi.Api;

namespace GrammarEngineApi
{
    public class TextFileSegmenter : IDisposable
    {
        private readonly GrammarEngine _gren;
        private IntPtr _hObject;

        public TextFileSegmenter(GrammarEngine gren, IntPtr hObject)
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
            int len;
            if ((len = GrammarApi.sol_FetchSentence(_hObject)) >= 0)
            {
                var b = new StringBuilder(len + 2);
                GrammarApi.sol_GetFetchedSentence(_hObject, b);
                return b.ToString();
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