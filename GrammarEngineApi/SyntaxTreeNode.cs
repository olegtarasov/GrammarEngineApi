using System;
using System.Collections.Generic;

namespace GrammarEngineApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Обращаю внимание, что дескриптор hNode НЕ НАДО ОСВОБОЖДАТЬ, поэтому IDisposable не нужен
    /// </remarks>
    public class SyntaxTreeNode
    {
        public readonly IntPtr HNode;
        private readonly GrammarEngine _gren;

        public List<SyntaxTreeNode> Leafs { get; }

        public SyntaxTreeNode(GrammarEngine gren, IntPtr hNode)
        {
            this._gren = gren;
            this.HNode = hNode;

            int nleaf = GrammarEngineApi.sol_CountLeafs(this.HNode);
            Leafs = new List<SyntaxTreeNode>();
            for (int i = 0; i < nleaf; ++i)
            {
                Leafs.Add(new SyntaxTreeNode(this._gren, GrammarEngineApi.sol_GetLeaf(this.HNode, i)));
            }
        }

        private Entry _entry;
        public Entry Entry => _entry ?? (_entry = new Entry(_gren, GrammarEngineApi.sol_GetNodeIEntry(_gren.GetEngineHandle(), HNode)));

        private List<CoordPair> _pairs;
        public IReadOnlyList<CoordPair> Pairs => _pairs ?? (_pairs = GetPairs());

        private string _word;
        public string Word => _word ?? (_word = GrammarEngineApi.sol_GetNodeContentsFX(HNode));

        public int SourcePosition => GrammarEngineApi.sol_GetNodePosition(HNode);

        public bool Contains(CoordPair p)
        {
            return GrammarEngineApi.sol_GetNodeCoordPair(HNode, p.CoordId, p.StateId) == 1;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return HNode.Equals(((SyntaxTreeNode)obj).HNode);
        }

        public int GetCoordState(int CoordID)
        {
            return GrammarEngineApi.sol_GetNodeCoordState(HNode, CoordID);
        }

        public override int GetHashCode()
        {
            return HNode.GetHashCode();
        }


        public int GetLinkType(int LeafIndex)
        {
            return GrammarEngineApi.sol_GetLeafLinkType(HNode, LeafIndex);
        }

        private List<CoordPair> GetPairs()
        {
            List<CoordPair> res = new List<CoordPair>();

            int n = GrammarEngineApi.sol_GetNodePairsCount(HNode);
            if (n > 0)
            {
                for (int i = 0; i < n; ++i)
                {
                    var p = new CoordPair(GrammarEngineApi.sol_GetNodePairCoord(HNode, i),
                        GrammarEngineApi.sol_GetNodePairState(HNode, i));
                    res.Add(p);
                }
            }

            return res;
        }

        public int GetVersionEntryID(int version_index)
        {
            return GrammarEngineApi.sol_GetNodeVerIEntry(_gren.GetEngineHandle(), HNode, version_index);
        }

        public override string ToString()
        {
            return Word;
        }


        public bool VersionContains(int version_index, CoordPair p)
        {
            return GrammarEngineApi.sol_GetNodeVerCoordPair(HNode, version_index, p.CoordId, p.StateId) == 1;
        }


        // Number of versions of morphological analysis
        public int VersionCount()
        {
            return GrammarEngineApi.sol_GetNodeVersionCount(_gren.GetEngineHandle(), HNode);
        }
    }
}