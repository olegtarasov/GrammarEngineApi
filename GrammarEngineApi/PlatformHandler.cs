using System;
using System.Text;
using System.Threading;
using NativeLibraryManager;

namespace GrammarEngineApi
{
    public static class PlatformHandler
    {
        public static readonly bool IsUnix = LibraryManager.GetPlatform() is Platform.Linux or Platform.MacOs;

        public static string GetNativeString(Action<IntPtr> unixAction, Action<StringBuilder> windowsAction, int len = 32)
        {
            if (IsUnix)
            {
                return GetUtf8String(unixAction, len);
            }

            return GetUnicodeString(windowsAction, len);
        }

        public static T UnixHandler<T>(Func<T> isUnix, Func<T> isOther)
        {
            if (IsUnix)
            {
                return isUnix();
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