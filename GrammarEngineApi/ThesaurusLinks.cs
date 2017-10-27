using System;

namespace GrammarEngineApi
{
    public class ThesaurusLinks : IDisposable
    {
        private readonly IntPtr _hEngine;
        private IntPtr _hList;

        public ThesaurusLinks(IntPtr hEngine, IntPtr hList)
        {
            this._hEngine = hEngine;
            this._hList = hList;
        }

        public int Count()
        {
            return GrammarApi.sol_LinksInfoCount(_hEngine, _hList);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int GetEntry2(int index)
        {
            return GrammarApi.sol_LinksInfoEKey2(_hEngine, _hList, index);
        }

        public int GetLinkId(int index)
        {
            return GrammarApi.sol_LinksInfoID(_hEngine, _hList, index);
        }

        public string GetTags(int index)
        {
            return GrammarApi.sol_LinksInfoTagsTxtFX(_hEngine, _hList, index);
        }

        //[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        protected virtual void Dispose(bool disposing)
        {
            if (_hList != IntPtr.Zero)
            {
                GrammarApi.sol_DeleteLinksInfo(_hEngine, _hList);
                _hList = IntPtr.Zero;
            }
        }
    }
}