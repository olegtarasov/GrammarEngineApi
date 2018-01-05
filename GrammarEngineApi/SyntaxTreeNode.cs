using System;
using System.Collections.Generic;
using System.Text;

namespace GrammarEngineApi
{
    /// <summary>
    ///     Syntax analysis node.
    /// </summary>
    public class SyntaxTreeNode : ILemmatizedToken
    {
        private readonly GrammarEngine _gren;
        private readonly IntPtr _hNode;

        private Entry _entry;

        private CoordPair[] _pairs;

        private string _word;

        public SyntaxTreeNode(GrammarEngine gren, IntPtr hNode)
        {
            _gren = gren;
            _hNode = hNode;

            int nleaf = GrammarApi.sol_CountLeafs(_hNode);
            Leafs = new SyntaxTreeNode[nleaf];
            for (int i = 0; i < nleaf; ++i)
            {
                Leafs[i] = new SyntaxTreeNode(_gren, GrammarApi.sol_GetLeaf(_hNode, i));
            }
        }

        /// <summary>
        ///     Grammar entry for the current node.
        /// </summary>
        public Entry GrammarEntry => _entry ?? (_entry = new Entry(_gren, GrammarApi.sol_GetNodeIEntry(_gren.GetEngineHandle(), _hNode)));

        /// <summary>
        ///     Indicates whether source word was lemmatized.
        /// </summary>
        public bool IsLemmatized => GrammarEntry.EntryExists;

        /// <summary>
        ///     Syntax leafs of this node.
        /// </summary>
        public SyntaxTreeNode[] Leafs { get; }

        /// <summary>
        ///     Coordinate pairs of this node.
        /// </summary>
        public IReadOnlyList<CoordPair> Pairs => _pairs ?? (_pairs = GetPairs());

        /// <summary>
        ///     The position of this node in a source sentense.
        /// </summary>
        public int SourcePosition => GrammarApi.sol_GetNodePosition(_hNode);

        /// <summary>
        ///     Always returns source word.
        /// </summary>
        public string SourceWord => _word ?? (_word = GetNodeContents(_hNode));

        /// <summary>
        ///     Gets lemmatized form of a word if <see cref="ILemmatizedToken.IsLemmatized" /> is <code>true</code> or
        ///     source word is it's <code>false</code>.
        /// </summary>
        public string Word => IsLemmatized ? GrammarEntry.Word.ToLower() : SourceWord;

        public bool ContainsPair(CoordPair p)
        {
            return GrammarApi.sol_GetNodeCoordPair(_hNode, p.CoordId, p.StateId) == 1;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return _hNode.Equals(((SyntaxTreeNode)obj)._hNode);
        }

        public int GetCoordState(int CoordID)
        {
            return GrammarApi.sol_GetNodeCoordState(_hNode, CoordID);
        }

        public override int GetHashCode()
        {
            return _hNode.GetHashCode();
        }


        public int GetLinkType(int LeafIndex)
        {
            return GrammarApi.sol_GetLeafLinkType(_hNode, LeafIndex);
        }

        public int GetVersionEntryID(int version_index)
        {
            return GrammarApi.sol_GetNodeVerIEntry(_gren.GetEngineHandle(), _hNode, version_index);
        }

        public override string ToString()
        {
            return $"{Word} [src: {SourceWord}][{(IsLemmatized ? "L" : "N")}]";
        }


        public bool VersionContains(int version_index, CoordPair p)
        {
            return GrammarApi.sol_GetNodeVerCoordPair(_hNode, version_index, p.CoordId, p.StateId) == 1;
        }

        // Number of versions of morphological analysis
        public int VersionCount()
        {
            return GrammarApi.sol_GetNodeVersionCount(_gren.GetEngineHandle(), _hNode);
        }

        private static string GetNodeContents(IntPtr hNode)
        {
            var b = new StringBuilder(32);
            GrammarApi.sol_GetNodeContents(hNode, b);
            return b.ToString();
        }

        private CoordPair[] GetPairs()
        {
            int n = GrammarApi.sol_GetNodePairsCount(_hNode);
            var res = new CoordPair[n];

            for (int i = 0; i < n; ++i)
            {
                int coord = GrammarApi.sol_GetNodePairCoord(_hNode, i);
                int state = GrammarApi.sol_GetNodePairState(_hNode, i);
                res[i] = new CoordPair(coord, state,
                    _gren.GetCoordName(coord),
                    _gren.GetCoordStateName(coord, state));
            }

            return res;
        }
    }
}