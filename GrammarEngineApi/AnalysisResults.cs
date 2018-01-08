using System;
using System.Collections.Generic;
using GrammarEngineApi.Api;

namespace GrammarEngineApi
{
    /// <summary>
    /// Morphological or syntax analysis result.
    /// </summary>
    public class AnalysisResults : IDisposable
    {
        private readonly IntPtr _hPack;
        private readonly SyntaxTreeNode[] _nodes;
        private readonly object _locker = new object();

        private bool _disposed = false;

        public AnalysisResults(GrammarEngine gren, IntPtr hPack, bool preserveMarkers = false)
        {
            _hPack = hPack;

            int n = GrammarApi.sol_CountRoots(_hPack, 0);
            if (n == 0)
            {
                _nodes = new SyntaxTreeNode[0];
                return;
            }

            int offset = preserveMarkers ? 0 : 1;
            
            _nodes = new SyntaxTreeNode[n - offset * 2];
            for (int i = offset; i < n - offset; i++)
            {
                _nodes[i - offset] = new SyntaxTreeNode(gren, GrammarApi.sol_GetRoot(_hPack, 0, i));
            }
        }

        public SyntaxTreeNode[] Nodes => _nodes;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IntPtr GetHandle()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Handle was already freed");
            }

            return _hPack;
        }

        //[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
            {
                return;
            }

            lock (_locker)
            {
                GrammarApi.sol_DeleteResPack(_hPack);
                _disposed = true;
            }
        }
    }
}