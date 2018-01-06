using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using GrammarEngineApi.Properties;

namespace GrammarEngineApi.Api
{
    public sealed partial class GrammarApi
    {
        private static readonly object _resourceLocker = new object();
        private static bool _grenLoaded = false;

        public static void LoadNativeLibrary()
        {
            if (_grenLoaded)
            {
                return;
            }

            lock (_resourceLocker)
            {
                if (_grenLoaded)
                {
                    return;
                }

                string dir = UnpackResources();
                string grenLib = Path.Combine(dir, GrenDllName);
                string lemLib = Path.Combine(dir, LemDllName);

                LoadLibrary(grenLib);
                LoadLibrary(lemLib);

                _grenLoaded = true;
            }
        }

        private static void LoadLibrary(string path)
        {
            _log.Info($"Directly loading {path}...");
            var result = LoadLibraryEx(path, IntPtr.Zero, LoadLibraryFlags.LOAD_LIBRARY_SEARCH_APPLICATION_DIR | LoadLibraryFlags.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS | LoadLibraryFlags.LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR | LoadLibraryFlags.LOAD_LIBRARY_SEARCH_SYSTEM32 | LoadLibraryFlags.LOAD_LIBRARY_SEARCH_USER_DIRS);
            _log.Info(result == IntPtr.Zero ? "FAILED!" : "Success");
        }

        private static string UnpackResources()
        {
            string curDir;
            var ass = Assembly.GetExecutingAssembly().Location;
            if (string.IsNullOrEmpty(ass))
            {
                curDir = Environment.CurrentDirectory;
            }
            else
            {
                curDir = Path.GetDirectoryName(ass);
            }

            _log.Info($"Unpacking native libs to {curDir}");

            UnpackFile(curDir, "sqlite.dll", Resources.sqlite);
            UnpackFile(curDir, "solarix_grammar_engine.dll", Resources.solarix_grammar_engine);
            UnpackFile(curDir, "lemmatizator.dll", Resources.lemmatizator);

            return curDir;
        }

        private static void UnpackFile(string curDir, string fileName, byte[] bytes)
        {
            var path = !string.IsNullOrEmpty(curDir) ? Path.Combine(curDir, fileName) : fileName;
            if (File.Exists(path))
            {
                return;
            }

            _log.Info($"{fileName} doesn't exist, unpacking.");

            File.WriteAllBytes(path, bytes);
        }

        #region LoadLibraryEx

        [System.Flags]
        private enum LoadLibraryFlags : uint
        {
            DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
            LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
            LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
            LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
            LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
            LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,
            LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000,
            LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
            LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
            LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400,
            LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

        #endregion
    }
}