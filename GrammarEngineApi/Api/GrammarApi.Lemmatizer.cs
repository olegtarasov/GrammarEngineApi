using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GrammarEngineApi.Api
{
    public sealed partial class GrammarApi
    {
        // http://www.solarix.ru/api/en/sol_LoadLemmatizator.shtml
        [DllImport(LemDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_LoadLemmatizatorW(string dbPath, LemmatizerFlags flags);

        // http://www.solarix.ru/api/en/sol_DeleteLemmatizator.shtml
        [DllImport(LemDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteLemmatizator(IntPtr hEngine);

        // http://www.solarix.ru/api/en/sol_GetLemma.shtml
        [DllImport(LemDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetLemmaW(IntPtr hEngine, string wordform, StringBuilder lemmaBuffer, int bufferLen);

        // http://www.solarix.ru/api/en/sol_GetLemmas.shtml
        [DllImport(LemDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_GetLemmasW(IntPtr hEngine, string wordform);

        [DllImport(LemDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_LemmatizePhraseW(IntPtr hEngine, string sentence, int flags, char wordSeparator);

        // http://www.solarix.ru/api/en/sol_CountLemmas.shtml
        [DllImport(LemDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountLemmas(IntPtr hList);

        // http://www.solarix.ru/api/en/sol_GetLemmaString.shtml
        [DllImport(LemDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetLemmaStringW(IntPtr hList, int index, StringBuilder lemmaBuffer, int bufferLen);

        // http://www.solarix.ru/api/en/sol_DeleteLemmas.shtml
        [DllImport(LemDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteLemmas(IntPtr hList);
    }
}