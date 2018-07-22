using System;
using System.Collections.Generic;
using GrammarEngineApi.Api;

namespace GrammarEngineApi
{
    public class WordProjection
    {
        private readonly GrammarEngine _engine;
        private readonly IntPtr _hList;
        private readonly int _idx;

        private CoordPair[] _pairs;

        internal WordProjection(GrammarEngine engine, IntPtr hList, int idx)
        {
            _engine = engine;
            _hList = hList;
            _idx = idx;

            Entry = _engine.GetEntry(GrammarApi.sol_GetIEntry(_hList, _idx));
        }

        public Entry Entry { get; set; }

        /// <summary>
        ///     Coordinate pairs of this node.
        /// </summary>
        public IReadOnlyList<CoordPair> Pairs => _pairs ?? (_pairs = GetPairs());


        private CoordPair[] GetPairs()
        {
            int cnt = GrammarApi.sol_GetProjCoordCount(_engine.GetEngineHandle(), _hList, _idx);
            var res = new CoordPair[cnt];

            for (int i = 0; i < cnt; ++i)
            {
                int coordId = GrammarApi.sol_GetProjCoordId(_engine.GetEngineHandle(), _hList, _idx, i);
                int stateId = GrammarApi.sol_GetProjStateId(_engine.GetEngineHandle(), _hList, _idx, i);

                res[i] = new CoordPair(coordId, stateId,
                    _engine.GetCoordName(coordId),
                    _engine.GetCoordStateName(coordId, stateId));
            }

            return res;
        }
    }
}