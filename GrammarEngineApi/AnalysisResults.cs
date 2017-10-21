using System;
using System.Collections.Generic;

namespace GrammarEngineApi
{
    public class AnalysisResults : IDisposable
    {
        private readonly AnalysisResultsSafeHandle _hPack; // дескриптор с блоком результатов, его нужно освобождать
        private readonly List<SyntaxTreeNode> _nodes;

        public AnalysisResults(GrammarEngine gren, IntPtr hPack)
        {
            _hPack = new AnalysisResultsSafeHandle(hPack);
            _nodes = new List<SyntaxTreeNode>();

            int n = GrammarEngineApi.sol_CountRoots(_hPack.DangerousGetHandle(), 0);
            for (int i = 0; i < n; ++i)
            {
                SyntaxTreeNode node = new SyntaxTreeNode(gren, GrammarEngineApi.sol_GetRoot(_hPack.DangerousGetHandle(), 0, i));
                _nodes.Add(node);
            }
        }

        public AnalysisResults(GrammarEngine gren, IntPtr hPack, bool releaseHandle)
        {
            _hPack = new AnalysisResultsSafeHandle(hPack, releaseHandle);
            _nodes = new List<SyntaxTreeNode>();

            int n = GrammarEngineApi.sol_CountRoots(_hPack.DangerousGetHandle(), 0);
            for (int i = 0; i < n; ++i)
            {
                SyntaxTreeNode node = new SyntaxTreeNode(gren, GrammarEngineApi.sol_GetRoot(_hPack.DangerousGetHandle(), 0, i));
                _nodes.Add(node);
            }
        }

        public IReadOnlyList<SyntaxTreeNode> Nodes => _nodes;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IntPtr GetHandle()
        {
            return _hPack.GetHandle();
        }

        public bool IsNull()
        {
            return _hPack.GetHandle() == IntPtr.Zero;
        }

        //[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        protected virtual void Dispose(bool disposing)
        {
            if (_hPack != null && !_hPack.IsInvalid)
            {
                _hPack.Dispose();
            }
        }
    }
}