using System;
using System.Text;
using System.Threading;
using NativeLibraryManager;

namespace GrammarEngineApi
{
    public static class PlatformHandler
    {
        public static readonly bool IsLinux = LibraryManager.GetPlatform() == Platform.Linux;

        public static string GetNativeString(Action<IntPtr> linuxAction, Action<StringBuilder> windowsAction, int len = 32)
        {
            if (IsLinux)
            {
                return GetUtf8String(linuxAction, len);
            }

            return GetUnicodeString(windowsAction, len);
        }

        public static T LinuxHandler<T>(Func<T> isLinux, Func<T> isOther)
        {
            if (IsLinux)
            {
                return isLinux();
            }

            return isOther();
        }


        public static string GetUnicodeString(Action<StringBuilder> action, int len = 32)
        {
            var buff = new StringBuilder(len);
            action(buff);
            return buff.ToString();
        }
        
        public static unsafe string GetUtf8String(Action<IntPtr> action, int len = 32)
        {
            var buff = GetBufferUtf8(len);
            fixed (void* bPtr = &buff[0])
            {
                action((IntPtr)bPtr);
            }

            return Utf8ToString(buff);
        }
        
        private static byte[] GetBufferUtf8(int len)
        {
            return new byte[len * 6];
        }
        
        private static string Utf8ToString(byte[] utf8)
        {
            for (var i = 0; i < utf8.Length; ++i)
            {
                if (utf8[i] == 0)
                {
                    return Encoding.UTF8.GetString(utf8, 0, i);
                }
            }

            throw new Exception();
        }
    }
}