using System;
using Microsoft.Win32.SafeHandles;

namespace GrammarEngineApi
{
    internal class AnalysisResultsSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        readonly bool _releaseHandle = true;

        public AnalysisResultsSafeHandle(IntPtr h)
            : base(true)
        {
            handle = h;
        }

        public AnalysisResultsSafeHandle(IntPtr h, bool releaseHandle)
            : base(true)
        {
            handle = h;
            _releaseHandle = releaseHandle;
        }

        public IntPtr GetHandle()
        {
            return handle;
        }

        //[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            if (_releaseHandle)
            {
                GrammarApi.sol_DeleteResPack(handle);
            }

            handle = IntPtr.Zero;
            return true;
        }
    }
}