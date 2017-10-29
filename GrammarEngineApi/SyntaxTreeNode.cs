using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            int nleaf = GrammarApi.sol_CountLeafs(this.HNode);
            Leafs = new List<SyntaxTreeNode>();
            for (int i = 0; i < nleaf; ++i)
            {
                Leafs.Add(new SyntaxTreeNode(this._gren, GrammarApi.sol_GetLeaf(this.HNode, i)));
            }
        }

        private Entry _entry;
        public Entry Entry => _entry ?? (_entry = new Entry(_gren, GrammarApi.sol_GetNodeIEntry(_gren.GetEngineHandle(), HNode)));

        private List<CoordPair> _pairs;
        public IReadOnlyList<CoordPair> Pairs => _pairs ?? (_pairs = GetPairs());

        private string _word;
        public string Word => _word ?? (_word = GetNodeContents(HNode));

        public int SourcePosition => GrammarApi.sol_GetNodePosition(HNode);

        public bool Contains(CoordPair p)
        {
            return GrammarApi.sol_GetNodeCoordPair(HNode, p.CoordId, p.StateId) == 1;
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
            return GrammarApi.sol_GetNodeCoordState(HNode, CoordID);
        }

        public override int GetHashCode()
        {
            return HNode.GetHashCode();
        }


        public int GetLinkType(int LeafIndex)
        {
            return GrammarApi.sol_GetLeafLinkType(HNode, LeafIndex);
        }

        private List<CoordPair> GetPairs()
        {
            List<CoordPair> res = new List<CoordPair>();

            int n = GrammarApi.sol_GetNodePairsCount(HNode);
            if (n > 0)
            {
                for (int i = 0; i < n; ++i)
                {
                    var p = new CoordPair(_gren, GrammarApi.sol_GetNodePairCoord(HNode, i),
                        GrammarApi.sol_GetNodePairState(HNode, i));
                    res.Add(p);
                }
            }

            return res;
        }

        public int GetVersionEntryID(int version_index)
        {
            return GrammarApi.sol_GetNodeVerIEntry(_gren.GetEngineHandle(), HNode, version_index);
        }

        public override string ToString()
        {
            return Word;
        }


        public bool VersionContains(int version_index, CoordPair p)
        {
            return GrammarApi.sol_GetNodeVerCoordPair(HNode, version_index, p.CoordId, p.StateId) == 1;
        }


        // Number of versions of morphological analysis
        public int VersionCount()
        {
            return GrammarApi.sol_GetNodeVersionCount(_gren.GetEngineHandle(), HNode);
        }

        public static string GetNodeContents(IntPtr hNode)
        {
            var b = new StringBuilder(32);
            GrammarApi.sol_GetNodeContents(hNode, b);
            return b.ToString();
        }
    }
}