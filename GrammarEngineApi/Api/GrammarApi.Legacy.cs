using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GrammarEngineApi.Api
{
    public sealed partial class GrammarApi
    {
        public static bool IsLinux
        {
            get
            {
#if NETSTANDARD || NETCORE || NETSTANDARD2_0 // должны быть определены в проекте через <DefineConstants>...</DefineConstants>
// https://github.com/dotnet/corefx/blob/master/src/System.Runtime.InteropServices.RuntimeInformation/ref/System.Runtime.InteropServices.RuntimeInformation.cs
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#else
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
#endif
            }
        }

        public static ulong sol_CountNGramsFX(IntPtr hEngine, int Type, int Order)
        {
            uint Hi = 0, Lo = 0;
            if (sol_CountNGrams(hEngine, Type, Order, out Hi, out Lo) == 0)
            {
                return 0;
            }

            ulong res = (((ulong)Hi) << 32) | Lo;
            return res;
        }

        public static string sol_GetFlexionHandlerWordformFX(
            IntPtr hEngine,
            IntPtr /*HFLEXIONTABLE*/ hFlex,
            string dims
        )
        {
            IntPtr wchar_ptr = sol_GetFlexionHandlerWordform(hEngine, hFlex, dims);

            if (wchar_ptr == (IntPtr)null)
            {
                return "";
            }

            string res = Marshal.PtrToStringUni(wchar_ptr);
            return res;
        }

        public static string sol_GetLongStringFX(IntPtr hString)
        {
            int l = sol_GetLongStringLenW(hString);
            StringBuilder b = new StringBuilder(l + 1);
            sol_GetLongStringW(hString, b);
            return b.ToString();
        }

        public static string sol_GetNodeMarkNameFX(IntPtr hNode, int mark_index)
        {
            StringBuilder b = new StringBuilder(64);
            sol_GetNodeMarkNameW(hNode, mark_index, b);
            return b.ToString();
        }

        public static string sol_GetPhraseTextFX(IntPtr hEngine, int PhraseId)
        {
            IntPtr wchar_ptr = sol_GetPhraseText(hEngine, PhraseId);

            if (wchar_ptr == (IntPtr)null)
            {
                return "";
            }

            string res = Marshal.PtrToStringUni(wchar_ptr);
            sol_Free(hEngine, wchar_ptr);
            return res;
        }

        public static string sol_GetStringFX(IntPtr hToks, int i)
        {
            int string_len = sol_GetStringLen(hToks, i);
            StringBuilder b = new StringBuilder(string_len + 1);
            sol_GetStringW(hToks, i, b);
            return b.ToString();
        }

        /// <summary>
        ///     Получение строки с номером версии грамматического движка в виде "XX.YY.ZZ битность"
        /// </summary>
        public static string sol_GetVersionFX(IntPtr hEngine)
        {
            int Major = 0, Minor = 0, Build = 0;
            int bits = sol_Bits();
            string bits_str = bits == 64 ? "x64" : "x86";
            sol_GetVersion(hEngine, ref Major, ref Minor, ref Build);
            return string.Format("{0}.{1}.{2} {3}", Major, Minor, Build, bits_str);
        }

        public static string sol_LinksInfoFlagsTxtFX(IntPtr hEngine, IntPtr /*HLINKSINFO*/ hList, int Index)
        {
            IntPtr wchar_ptr = sol_LinksInfoFlagsTxt(hEngine, hList, Index);

            if (wchar_ptr == (IntPtr)null)
            {
                return "";
            }

            string res = Marshal.PtrToStringUni(wchar_ptr);
            return res;
        }

        public static string sol_LinksInfoTagsTxtFX(IntPtr hEngine, IntPtr /*HLINKSINFO*/ hList, int Index)
        {
            IntPtr wchar_ptr = sol_LinksInfoTagsTxt(hEngine, hList, Index);

            if (wchar_ptr == (IntPtr)null)
            {
                return "";
            }

            string res = Marshal.PtrToStringUni(wchar_ptr);
            return res;
        }

        public static int[] sol_ListEntriesFX(IntPtr hEngine, int Flags, int EntryType, string Mask, int Language, int Class)
        {
            IntPtr hList = sol_ListEntries(hEngine, Flags, EntryType, Mask, Language, Class);
            if (hList == (IntPtr)null)
            {
                return new int[0];
            }

            int n = sol_CountInts(hList);
            int[] res = new int[n];

            for (int i = 0; i < n; ++i)
            {
                res[i] = sol_GetInt(hList, i);
            }

            sol_DeleteInts(hList);

            return res;
        }

        public static IntPtr sol_ProjectMisspelledWordFX(IntPtr hEngine, string Word, int AllowDynforms, int nmaxmiss)
        {
            if (IsLinux)
            {
                byte[] word8 = Encoding.UTF8.GetBytes(Word);
                return sol_ProjectMisspelledWord8(hEngine, word8, AllowDynforms, nmaxmiss);
            }

            return sol_ProjectMisspelledWord(hEngine, Word, AllowDynforms, nmaxmiss);
        }

        public static string sol_RestoreCasingFX(IntPtr hEngine, string Word, int id_entry)
        {
            StringBuilder b = new StringBuilder(32);
            b.Append(Word);
            sol_RestoreCasing(hEngine, b, id_entry);
            return b.ToString();
        }

        public static int[] sol_SeekThesaurusFX(IntPtr hEngine, int EntryID, bool Synonyms, bool Grammar_Links, bool Translation, bool Semantics, int nJumps)
        {
            IntPtr hList = sol_SeekThesaurus(hEngine, EntryID, Synonyms ? 1 : 0, Grammar_Links ? 1 : 0, Translation ? 1 : 0, Semantics ? 1 : 0, nJumps);
            if (hList != (IntPtr)0)
            {
                int n = sol_CountInts(hList);
                int[] res = new int[n];
                for (int i = 0; i < n; ++i)
                {
                    res[i] = sol_GetInt(hList, i);
                }

                sol_DeleteInts(hList);
                return res;
            }

            return new int[0];
        }
    }
}