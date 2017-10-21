using System;

namespace GrammarEngineApi
{
    public class WordProjections : IDisposable
    {
        private readonly IntPtr _hEngine;
        private IntPtr _hList;

        public WordProjections(IntPtr hEngine, IntPtr hList)
        {
            _hEngine = hEngine;
            _hList = hList;
        }

        public int Count { get { return _hList == IntPtr.Zero ? 0 : GrammarEngineApi.sol_CountProjections(_hList); } }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int GetCoordState(int index, int coordId)
        {
            return GrammarEngineApi.sol_GetProjCoordState(_hEngine, _hList, index, coordId);
        }

        public int GetEntryKey(int i)
        {
            return GrammarEngineApi.sol_GetIEntry(_hList, i);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_hList != IntPtr.Zero)
            {
                GrammarEngineApi.sol_DeleteProjections(_hList);
                _hList = IntPtr.Zero;
            }
        }
    }
}