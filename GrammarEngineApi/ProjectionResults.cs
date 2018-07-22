using System;
using System.Collections;
using System.Collections.Generic;
using GrammarEngineApi.Api;

namespace GrammarEngineApi
{
    public class ProjectionResults : IDisposable, IEnumerable<WordProjection>
    {
        private readonly GrammarEngine _engine;
        private IntPtr _hList;

        public ProjectionResults(GrammarEngine engine, IntPtr hList)
        {
            _engine = engine;
            _hList = hList;
        }

        public int Count { get { return _hList == IntPtr.Zero ? 0 : GrammarApi.sol_CountProjections(_hList); } }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int GetCoordState(int index, int coordId)
        {
            return GrammarApi.sol_GetProjCoordState(_engine.GetEngineHandle(), _hList, index, coordId);
        }

        public int GetEntryKey(int i)
        {
            return GrammarApi.sol_GetIEntry(_hList, i);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_hList != IntPtr.Zero)
            {
                GrammarApi.sol_DeleteProjections(_hList);
                _hList = IntPtr.Zero;
            }
        }

        public IEnumerator<WordProjection> GetEnumerator()
        {
            return new PorjectionsEnumerator(_engine, _hList, Count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private struct PorjectionsEnumerator : IEnumerator<WordProjection>
        {
            private readonly GrammarEngine _engine;
            private readonly int _count;
            private readonly IntPtr _hList;

            private int _cur;

            public PorjectionsEnumerator(GrammarEngine engine, IntPtr hList, int count)
            {
                _hList = hList;
                _count = count;
                _engine = engine;
                _cur = -1;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_cur == _count - 1)
                {
                    return false;
                }

                _cur++;

                return true;
            }

            public void Reset()
            {
                _cur = 0;
            }

            public WordProjection Current => new WordProjection(_engine, _hList, _cur);

            object IEnumerator.Current => Current;
        }
    }
}