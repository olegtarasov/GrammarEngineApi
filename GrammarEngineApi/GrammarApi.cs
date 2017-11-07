// .NET обертка для вызова функций процедурного API грамматического словаря
// Для более удобной работы в ООП следует использовать сборку gren_fx2 и дополнительные классы в ней.
//
// Информация по API:
// http://www.solarix.ru/api/ru/list.shtml
// http://www.solarix.ru/for_developers/api/grammar-engine-api.shtml
// http://www.solarix.ru/for_developers/api/dotnet_assembly.shtml
//
// CHANGELOG:
// 17.07.2008 - перевод вызовов WinDLL API на stdcall и соответствующий переход здесь на CallingConvention.StdCall
// 25.07.2008 - добавлены sol_FindClass, sol_FindEnum, sol_FindEnumState, sol_EnumInClass
// 27.07.2008 - добавлена sol_SeekNGrams
// 01.08.2008 - все возвращаемые процедурами DLL значения с bool заменены на int
// 04.04.2008 - изменен набор управляющих констант для API генератора текста
// 15.11.2008 - добавлена sol_LoadKnowledgeBase, добавлен флаг FG_PEDANTIC_ANALYSIS
// 15.11.2008 - обертка переделана на работу с полной версией поискового движка
// 16.11.2008 - добавлены обертки для API поисковика
// 13.02.2009 - добавлены обертки API SentenceBroker'а
// 08.04.2009 - из API удалена sol_PreloadCaches
// 21.06.2009 - переделки в алгоритме и константах синонимизатора
// 20.07.2009 - переделан API доступа к базе N-грамм
// 22.12.2009 - из сборки враппера убран инферфейс поискового движка и синонимизатора
// 02.08.2010 - добавлена sol_SaveDictionary
// 06.08.2010 - добавлены обертки sol_GetPhraseLanguage, sol_GetPhraseClass и sol_AddWord
// 19.08.2010 - добавлены обертки sol_LinksInfoFlagsTxt, sol_SetLinkFlags
// 04.09.2010 - добавлены обертки sol_GetError, sol_ClearError
// 19.06.2011 - добавлены sol_GetTranslationLog и sol_GetTranslationLogFx для вывода отладочной
//              трассировки в переводчике.
// 02.10.2011 - изменения в сигнатуре sol_MorphologyAnalysis и sol_SyntaxAnalysis
// 22.10.2011 - переделки в API sol_Tokenize -> sol_TokenizeW
// 13.11.2011 - добавлены обертки sol_GetCoordNameFX и sol_GetCoordStateNameFX
// 09.01.2012 - изменения в сигнатуре функции sol_Syllabs
// 07.01.2013 - добавлен флаг SOL_GREN_MODEL для включения вероятностной модели
// 12.09.2017 - портирование на .NET Core под Linux
//
// CD->30.01.2008
// LC->12.09.2017
// --------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using GrammarEngineApi.Properties;
using log4net;

namespace GrammarEngineApi
{
    /// <summary>
    /// Low-level API
    /// </summary>
    public sealed class GrammarApi
    {
        //private const string gren_dll = "libgren.so";
        private const string gren_dll = "solarix_grammar_engine.dll";

        private static readonly ILog _log = LogManager.GetLogger(typeof(GrammarEngine));

        // -----------------------------

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


        // This is a simple and helpful wrapper for low-level sol_CountNGrams. It returns the 64 bit count value.
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


        public static List<string> sol_GenerateWordformsFX(IntPtr hEngine, int EntryID, List<int> CoordID, List<int> StateID)
        {
            int npairs = CoordID.Count;
            int[] pairs = new int[npairs * 2];
            for (int i = 0, j = 0; i < npairs; ++i)
            {
                pairs[j++] = CoordID[i];
                pairs[j++] = StateID[i];
            }

            List<string> res = new List<string>();
            IntPtr hStr = sol_GenerateWordforms(hEngine, EntryID, npairs, pairs);
            if (hStr != (IntPtr)0)
            {
                int nstr = sol_CountStrings(hStr);
                for (int k = 0; k < nstr; ++k)
                {
                    res.Add(sol_GetStringFX(hStr, k));
                }

                sol_DeleteStrings(hStr);
            }

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

        public static void LoadNativeLibrary()
        {
            string dir = UnpackResources();
            string lib = Path.Combine(dir, "solarix_grammar_engine.dll");

            _log.Info($"Directly loading {lib}...");
            var result = LoadLibraryEx(lib, IntPtr.Zero, LoadLibraryFlags.LOAD_LIBRARY_SEARCH_APPLICATION_DIR | LoadLibraryFlags.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS | LoadLibraryFlags.LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR | LoadLibraryFlags.LOAD_LIBRARY_SEARCH_SYSTEM32 | LoadLibraryFlags.LOAD_LIBRARY_SEARCH_USER_DIRS);
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

            UnpackFile(curDir, "libdescr.dll", Resources.libdesr);
            UnpackFile(curDir, "sqlite.dll", Resources.sqlite);
            UnpackFile(curDir, "solarix_grammar_engine.dll", Resources.solarix_grammar_engine);

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

        #region low-level API for Windows

        [System.Flags]
        enum LoadLibraryFlags : uint
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
        static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

        /// <summary>
        ///     Создание экземпляра грамматического движка в памяти.
        ///     При непустом аргументе dictConfigPath будет также подключена словарная база.
        ///     Данная функция загружает весь лексикон сразу в память. Если возникающий при этом расход памяти и
        ///     задержка инициализации неприемлемы, следует использовать функцию sol_CreateGrammarEngineExW с
        ///     указанием флага EngineInstanceFlags.SOL_GREN_LAZY_LEXICON.
        ///     Онлайн-документация: http://www.solarix.ru/api/ru/sol_CreateGrammarEngine.shtml
        /// </summary>
        /// <param name="DictPath">Пустая строка или путь к файлу конфигурации словарной базы (dictionary.xml)</param>
        /// <returns>Возвращается дескриптор созданного движка или IntPtr.Zero в случе ошибки</returns>
        /// <seealso cref="sol_CreateGrammarEngineExW">sol_CreateGrammarEngineExW</seealso>
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_CreateGrammarEngineW(string dictConfigPath);

        /// <summary>
        ///     Создание экземпляра грамматического движка в памяти.
        /// </summary>
        /// <param name="dictConfigPath">Путь к файлу конфигурации словаря (обычно dictionary.xml)</param>
        /// <param name="instanceFlags">Флаги для задания режима работы движка</param>
        /// <returns></returns>
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_CreateGrammarEngineExW(string dictConfigPath, EngineInstanceFlags instanceFlags);

        /// <summary>
        ///     Уничтожение экземпляра грамматического движка в памяти, освобождение всех выделенных ресурсов.
        ///     Онлайн-документация: http://www.solarix.ru/api/ru/sol_DeleteGrammarEngine.shtml
        /// </summary>
        /// <param name="hEngine">
        ///     Дескриптор экземпляра, возвращенный при вызове sol_CreateGrammarEngineW или
        ///     sol_CreateGrammarEngineExW
        /// </param>
        /// <returns>1 при нормальном уничтожении, 0 при возникновении ошибки (некорректный дескриптор экземпляра)</returns>
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteGrammarEngine(IntPtr hEngine);

        // http://www.solarix.ru/api/ru/sol_LoadDictionary.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LoadDictionaryW(IntPtr hEngine, string dictPath);

        // http://www.solarix.ru/api/ru/sol_LoadDictionary.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LoadDictionaryExW(IntPtr hEngine, string dictPath, EngineInstanceFlags flags);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LoadDictionary8(IntPtr hEngine, byte[] DictionaryPath8);

        /// <summary>
        ///     Возвращает компоненты версии скомпилированного движка (через ссылочные аргументы).
        /// </summary>
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetVersion(IntPtr hEngine, ref int majorNumber, ref int minorNumber, ref int buildNumber);

        /// <summary>
        ///     Возвращает номер версии загруженного словаря.
        /// </summary>
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DictionaryVersion(IntPtr hEngine);

        // http://www.solarix.ru/api/ru/sol_CountEntries.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountEntries(IntPtr hEngine);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountForms(IntPtr hEngine);

        // http://www.solarix.ru/api/ru/sol_CountLinks.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountLinks(IntPtr hEngine);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern void sol_SetLanguage(IntPtr hEngine, int LanguageID);

        // http://www.solarix.ru/api/ru/sol_MaxLexemLen.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_MaxLexemLen(IntPtr hEngine);

        // http://www.solarix.ru/api/ru/sol_FindEntry.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindEntry(IntPtr hEngine, string name, int partOfSpeechId, int languageId);

        // http://www.solarix.ru/api/ru/sol_FindClass.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindClass(IntPtr hEngine, string partOfSpeechName);

        // http://www.solarix.ru/api/ru/sol_FindEnum.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindEnum(IntPtr hEngine, string attributeName);

        // http://www.solarix.ru/api/ru/sol_FindEnumState.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindEnumState(IntPtr hEngine, int attributeId, string stateName);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_EnumInClass(IntPtr hEngine, int partOfSpeechId, int attributeId);


        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindTagW(IntPtr hEngine, string tagName);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindTagValueW(IntPtr hEngine, int tagID, string valueName);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_FindStringsEx(IntPtr hEngine, string name, int Allow_Dynforms,
                                                      int Synonyms, int Grammar_Links, int Translations, int Semantics, int nJumps);

        // http://www.solarix.ru/api/ru/sol_CountStrings.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountStrings(IntPtr hStr);

        // http://www.solarix.ru/api/ru/sol_GetStringLen.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetStringLen(IntPtr hStr, int i);

        // http://www.solarix.ru/api/ru/sol_GetString.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetStringW(IntPtr hStr, int i, StringBuilder buffer);

        // http://www.solarix.ru/api/ru/sol_DeleteStrings.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteStrings(IntPtr hStr);


        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountNodeMarks(IntPtr hNode);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeMarkNameW(IntPtr hNode, int mark_index, StringBuilder name_buffer);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_SerializeNodeMark(IntPtr hEngine, IntPtr hNode, int mark_index, int format);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetLongStringLenW(IntPtr hString);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetLongStringW(IntPtr hString, StringBuilder buffer);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteLongString(IntPtr hString);


        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Stemmer(IntPtr hEngine, string Word);

        // http://www.solarix.ru/api/ru/sol_GetEntryName.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetEntryName(IntPtr hEngine, int EntryID, StringBuilder Result);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetEntryName8(IntPtr hEngine, int EntryID, byte[] ResultUtf8);

        // http://www.solarix.ru/api/ru/sol_GetEntryClass.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetEntryClass(IntPtr hEngine, int EntryID);

        // http://www.solarix.ru/api/ru/sol_GetEntryCoordState.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetEntryCoordState(IntPtr hEngine, int EntryID, int CategoryID);

        // http://www.solarix.ru/api/ru/sol_FindEntryCoordPair.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindEntryCoordPair(IntPtr hEngine, int EntryID, int CategoryID, int StateID);

        // http://www.solarix.ru/api/ru/sol_GetClassName.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetClassName(IntPtr hEngine, int ClassIndex, StringBuilder Result);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetClassName8(IntPtr hEngine, int ClassIndex, byte[] buffer);


        // http://www.solarix.ru/api/ru/sol_GetCoordType.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetCoordType(IntPtr hEngine, int CoordID, int PartOfSpeechID);

        // http://www.solarix.ru/api/ru/sol_GetCoordName.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetCoordName(IntPtr hEngine, int CoordID, StringBuilder Result);

        // http://www.solarix.ru/api/ru/sol_GetCoordStateName.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetCoordStateName(
            IntPtr hEngine,
            int CoordID,
            int StateID,
            StringBuilder Result
        );

        // http://www.solarix.ru/api/en/sol_CountCoordStates.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountCoordStates(IntPtr h, int CoordID);

        // http://www.solarix.ru/api/en/sol_GetNounGender.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNounGender(IntPtr hStr, int EntryIndex);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNounForm(IntPtr hEngine, int EntryIndex, int Number, int Case, StringBuilder Result);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetVerbForm(IntPtr hEngine, int EntryIndex,
                                                 int Number, int Gender, int Tense, int Person, StringBuilder Result);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetAdjectiveForm(IntPtr hEngine, int EntryIndex,
                                                      int Number, int Gender, int Case, int Anim, int Shortness, int Compar_Form, StringBuilder Result);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CorrNounNumber(IntPtr hEngine, int EntryIndex,
                                                    int Value, int Case, int Anim, StringBuilder Result);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CorrVerbNumber(IntPtr hEngine, int EntryIndex,
                                                    int Value, int Gender, int Tense, StringBuilder Result);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CorrAdjNumber(IntPtr hEngine, int EntryIndex,
                                                   int Value, int Case, int Gender, int Anim, StringBuilder Result);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Syllabs(IntPtr hEngine, string OrgWord, char SyllabDelimiter, StringBuilder Result, int LanguageID);


        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindWord(IntPtr hEngine, string Word,
                                              IntPtr EntryIndex, IntPtr Form, IntPtr Class);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_SeekWord(IntPtr hEngine, string Word, int Allow_Dynforms);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_TranslateToNoun(IntPtr hEngine, int EntryIndex);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_TranslateToInfinitive(IntPtr hEngine, int EntryIndex);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_TranslateToBases(IntPtr hEngine, string Word, int AllowDynforms);

        // http://www.solarix.ru/api/ru/sol_ProjectWord.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_ProjectWord(IntPtr hEngine, string Word, int AllowDynforms);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_ProjectMisspelledWord(IntPtr hEngine, string Word, int AllowDynforms, int nmaxmiss);

        [DllImport(gren_dll)]
        public static extern IntPtr sol_ProjectMisspelledWord8(IntPtr hEngine, byte[] Word, int AllowDynforms, int nmaxmiss);


        // http://www.solarix.ru/api/ru/sol_CountProjections.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountProjections(IntPtr hList);

        // http://www.solarix.ru/api/ru/sol_DeleteProjections.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern void sol_DeleteProjections(IntPtr hList);

        // http://www.solarix.ru/api/ru/sol_GetIEntry.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetIEntry(IntPtr hList, int Index);

        // http://www.solarix.ru/api/ru/sol_GetProjCoordState.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetProjCoordState(IntPtr hEngine, IntPtr hList, int Index, int Coord);


        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetProjCoordCount(IntPtr hEngine, IntPtr hList, int Index);


        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetProjCoordId(IntPtr hEngine, IntPtr hList, int ProjIndex, int TagIndex);


        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetProjStateId(IntPtr hEngine, IntPtr hList, int ProjIndex, int TagIndex);


        // http://www.solarix.ru/api/ru/sol_GetNounGender.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern void sol_DeleteResPack(IntPtr hPack);

        // http://www.solarix.ru/api/ru/sol_CountGrafs.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountGrafs(IntPtr hPack);

        // http://www.solarix.ru/api/ru/sol_CountRoots.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountRoots(IntPtr hPack, int iGraf);

        // http://www.solarix.ru/api/ru/sol_GetRoot.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_GetRoot(IntPtr hPack, int iGraf, int iRoot);

        // http://www.solarix.ru/api/ru/sol_CountLeafs.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountLeafs(IntPtr hNode);

        // http://www.solarix.ru/api/ru/sol_GetLeaf.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_GetLeaf(IntPtr hNode, int iLeaf);

        // http://www.solarix.ru/api/ru/sol_GetNodeVersionCount.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVersionCount(IntPtr hEngine, IntPtr hNode);

        // http://www.solarix.ru/api/ru/sol_GetLeafLinkType.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetLeafLinkType(IntPtr hNode, int iLeaf);


        // http://www.solarix.ru/api/ru/sol_GetNodeIEntry.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeIEntry(IntPtr hEngine, IntPtr hNode);

        // http://www.solarix.ru/api/ru/sol_GetNodeVerIEntry.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVerIEntry(IntPtr hEngine, IntPtr hNode, int ialt);

        // http://www.solarix.ru/api/ru/sol_GetNodeContents.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void sol_GetNodeContents(IntPtr hNode, StringBuilder Result);

        // http://www.solarix.ru/api/ru/sol_GetNodePosition.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodePosition(IntPtr hNode);

        // http://www.solarix.ru/api/ru/sol_GetNodeCoordState.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeCoordState(IntPtr hNode, int CoordID);

        // http://www.solarix.ru/api/ru/sol_GetNodeVerCoordState.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVerCoordState(IntPtr hNode, int iver, int CoordID);

        // http://www.solarix.ru/api/ru/sol_GetNodeCoordPair.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeCoordPair(IntPtr hNode, int CoordID, int StateID);

        // http://www.solarix.ru/api/ru/sol_GetNodeVerCoordPair.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVerCoordPair(IntPtr hNode, int iver, int CoordID, int StateID);

        // http://www.solarix.ru/api/ru/sol_GetNodePairsCount.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodePairsCount(IntPtr hNode);

        // http://www.solarix.ru/api/ru/sol_GetNodeVerPairsCount.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVerPairsCount(IntPtr hNode, int iver);

        // http://www.solarix.ru/api/ru/sol_GetNodePairCoord.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodePairCoord(IntPtr hNode, int ipair);

        // http://www.solarix.ru/api/ru/sol_GetNodeVerPairCoord.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVerPairCoord(IntPtr hNode, int iver, int ipair);

        // http://www.solarix.ru/api/ru/sol_GetNodePairState.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodePairState(IntPtr hNode, int ipair);

        // http://www.solarix.ru/api/ru/sol_GetNodeVerPairState.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVerPairState(IntPtr hNode, int iver, int ipair);

        // http://www.solarix.ru/api/ru/sol_CountInts.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountInts(IntPtr hInts);

        // http://www.solarix.ru/api/ru/sol_GetInt.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetInt(IntPtr hInts, int i);

        // http://www.solarix.ru/api/ru/sol_DeleteInts.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern void sol_DeleteInts(IntPtr hInts);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_SeekThesaurus(IntPtr hEngine, int EntryIndex, int Synonyms, int Grammar_Links, int Translation, int Semantics, int nJumps);

        // *********************************************************************************
        // N-grams database functions
        // Overview on Russian: http://www.solarix.ru/for_developers/api/ngrams-api.shtml
        // *********************************************************************************

        // http://www.solarix.ru/api/en/sol_CountNGrams.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountNGrams(IntPtr hEngine, int Type, int Order, out uint Hi, out uint Lo);

        // http://www.solarix.ru/api/ru/sol_Seek1Grams.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Seek1Grams(IntPtr hEngine, int Type, string word1);

        // http://www.solarix.ru/api/ru/sol_Seek2Grams.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Seek2Grams(IntPtr hEngine, int Type, string word1, string word2);

        // http://www.solarix.ru/api/ru/sol_Seek3Grams.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Seek3Grams(IntPtr hEngine, int Type, string word1, string word2, string word3);

        // http://www.solarix.ru/api/ru/sol_Seek4Grams.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Seek4Grams(IntPtr hEngine, int Type, string word1, string word2, string word3, string word4);

        // http://www.solarix.ru/api/ru/sol_Seek5Grams.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Seek5Grams(IntPtr hEngine, int Type, string word1, string word2, string word3, string word4, string word5);

        // --- SentenceBroker API --- 

        // http://www.solarix.ru/api/ru/sol_CreateSentenceBroker.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_CreateSentenceBroker(IntPtr hEngine, string Filename, string DefaultCodepage, int language);

        // http://www.solarix.ru/api/ru/sol_CreateSentenceBrokerMem.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_CreateSentenceBrokerMemW(IntPtr hEngine, string Text, int language);

        // http://www.solarix.ru/api/ru/sol_FetchSentence.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FetchSentence(IntPtr hBroker);

        // http://www.solarix.ru/api/ru/sol_GetFetchedSentenceLen.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetFetchedSentenceLen(IntPtr hBroker);

        // http://www.solarix.ru/api/ru/sol_GetFetchedSentence.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetFetchedSentence(IntPtr hBroker, StringBuilder Buffer);

        // http://www.solarix.ru/api/ru/sol_DeleteSentenceBroker.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void sol_DeleteSentenceBroker(IntPtr hBroker);

        // http://www.solarix.ru/api/ru/sol_Tokenize.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_TokenizeW(IntPtr hEngine, string Text, int language);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Bits();

        // http://www.solarix.ru/api/ru/sol_ListLinksTxt.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_ListLinksTxt(IntPtr hEng, int Key1, int LinkType, int Flags);

        // http://www.solarix.ru/api/ru/sol_DeleteLinksInfo.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteLinksInfo(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList);

        // http://www.solarix.ru/api/ru/sol_LinksInfoCount.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LinksInfoCount(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList);

        // http://www.solarix.ru/api/ru/sol_LinksInfoType.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LinksInfoType(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList, int Index);

        // http://www.solarix.ru/api/ru/sol_LinksInfoID.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LinksInfoID(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList, int Index);

        // http://www.solarix.ru/api/ru/sol_LinksInfoEKey1.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LinksInfoEKey1(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList, int Index);

        // http://www.solarix.ru/api/ru/sol_LinksInfoEKey2.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LinksInfoEKey2(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList, int Index);

        // http://www.solarix.ru/api/ru/sol_LinksInfoTagsTxt.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern /*wchar_t**/ IntPtr sol_LinksInfoTagsTxt(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList, int Index);

        // http://www.solarix.ru/api/ru/sol_LinksInfoFlagsTxt.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern /*wchar_t**/ IntPtr sol_LinksInfoFlagsTxt(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList, int Index);

        // http://www.solarix.ru/api/ru/sol_DeleteLink.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteLink(IntPtr hEng, int LinkID, int LinkType);

        // http://www.solarix.ru/api/ru/sol_AddLink.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_AddLink(IntPtr hEng, int LinkType, int IE1, int LinkCode, int IE2, string Tags);


        // http://www.solarix.ru/api/ru/sol_FindLanguage.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindLanguage(IntPtr hEng, string LanguageName);

        /// <summary>
        ///     Find a phrase given a phrase text.
        ///     <para>http://www.solarix.ru/api/en/sol_FindPhrase.shtml</para>
        /// </summary>
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindPhrase(IntPtr hEng, string Phrase, int Flags);

        /// <summary>
        ///     Create new phrase entry in lexicon.
        ///     <para>http://www.solarix.ru/api/en/sol_AddPhrase.shtml</para>
        /// </summary>
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_AddPhrase(IntPtr hEng, string Phrase, int LanguageID, int ClassID, int ProcessingFlags);

        /// <summary>
        ///     Retrieve the text contents of the phrase.
        ///     <para>http://www.solarix.ru/api/en/sol_GetPhraseText.shtml</para>
        /// </summary>
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern /*wchar_t**/ IntPtr sol_GetPhraseText(IntPtr hEng, int PhraseId);

        /// <summary>
        ///     Get the language ID if it was set for the phrase
        ///     <para>http://www.solarix.ru/api/en/sol_GetPhraseLanguage.shtml</para>
        /// </summary>
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetPhraseLanguage(IntPtr hEng, int PhraseId);

        /// <summary>
        ///     Get the part of speech ID if it was set for the phrase
        ///     <para>http://www.solarix.ru/api/en/sol_GetPhraseClass.shtml</para>
        /// </summary>
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetPhraseClass(IntPtr hEng, int PhraseId);


        // http://www.solarix.ru/api/ru/sol_Free.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Free(IntPtr hEngine, IntPtr ptr);


        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern /* wchar_t* */ IntPtr sol_NormalizePhraseW(IntPtr hEng, IntPtr hLinkages);

        // http://www.solarix.ru/api/ru/sol_AddWord.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_AddWord(IntPtr hEng, string Txt);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_ProcessPhraseEntry(
            IntPtr hEngine,
            int PhraseId,
            string Scenario,
            int Language,
            char DelimiterChar
        );

        // http://www.solarix.ru/api/ru/sol_DeletePhrase.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeletePhrase(IntPtr hEngine, int PhraseId);


        // http://www.solarix.ru/api/ru/sol_ListEntries.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr /*IntPtr_INTARRAY*/ sol_ListEntries(IntPtr hEngine, int Flags, int EntryType, string Mask, int Language, int PartOfSpeech);


        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_ListEntryForms(IntPtr hEngine, int EntryKey);


        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_SaveDictionary(IntPtr hEngine, int Flags, string Folder);


        /// <summary>
        ///     Выполнение морфологического разбора для слов в указанном предложении.
        ///     В том числе может выполнять частеречную разметку с использованием вероятностной модели языка, если она
        ///     подключена в словаре.
        ///     http://www.solarix.ru/api/ru/sol_MorphologyAnalysis.shtml
        /// </summary>
        /// <returns>
        ///     Возвращается дескриптор структуры с результатами разбора (см. процедуру
        ///     <see cref="sol_CountRoots">sol_CountRoots</see> и другие)
        /// </returns>
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr /*IntPtr_RESPACK*/ sol_MorphologyAnalysis(
            IntPtr hEngine,
            string sentence,
            MorphologyFlags flags,
            SyntaxFlags unusedFlags,
            int constraints,
            int languageId
        );

        /// <summary>
        ///     Выполнение синтаксического разбора предложения.
        ///     http://www.solarix.ru/api/ru/sol_SyntaxAnalysis.shtml
        /// </summary>
        /// <returns>
        ///     Возвращается дескриптор структуры с результатами разбора (см. процедуру
        ///     <see cref="sol_CountRoots">sol_CountRoots</see> и другие)
        /// </returns>
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_SyntaxAnalysis(
            IntPtr hEngine,
            string sentence,
            MorphologyFlags flags,
            SyntaxFlags unusedFlags,
            int constraints,
            int languageID
        );


        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr /*HFLEXIONS*/ sol_FindFlexionHandlers(IntPtr hEnging, string WordBasicForm, int SearchEntries);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteFlexionHandlers(IntPtr hEngine, IntPtr /*HFLEXIONS*/ hFlexs);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountEntriesInFlexionHandlers(IntPtr hEngine, IntPtr /*HFLEXIONS*/ hFlexs);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountParadigmasInFlexionHandlers(IntPtr hEngine, IntPtr /*HFLEXIONS*/ hFlexs);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetEntryInFlexionHandlers(IntPtr hEngine, IntPtr /*HFLEXIONS*/ hFlexs, int Index);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetParadigmaInFlexionHandlers(IntPtr hEngine, IntPtr /*HFLEXIONS*/ hFlexs, int Index, StringBuilder ParadigmaName);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr /*HFLEXIONTABLE*/ sol_BuildFlexionHandler(
            IntPtr hEngine,
            IntPtr /*HFLEXIONS*/ hFlexs,
            string ParadigmaName,
            int EntryIndex
        );

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr /*const wchar_t* */ sol_GetFlexionHandlerWordform(
            IntPtr hEngine,
            IntPtr /*HFLEXIONTABLE*/ hFlex,
            string dims
        );


        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteFlexionHandler(IntPtr hEngine, IntPtr /*HFLEXIONTABLE*/ hFlex);

        // http://www.solarix.ru/api/ru/sol_SetLinkFlags.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_SetLinkFlags(IntPtr hEngine, int id_link, string Flags);

        // http://www.solarix.ru/api/ru/sol_SetLinkTags.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_SetLinkTags(IntPtr hEngine, int LinkType, int id_link, string Tags);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_SetPhraseNote(IntPtr hEngine, int te_id, string Name, string Value);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_ReserveLexiconSpace(IntPtr hEngine, int n);

        // http://www.solarix.ru/api/ru/sol_GetError.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetError(IntPtr hEngine, StringBuilder buffer, int buffer_len);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetError8(IntPtr hEngine, byte[] buffer, int buffer_len);

        // http://www.solarix.ru/api/ru/sol_GetErrorLen.shtml
        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetErrorLen(IntPtr hEngine);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetErrorLen8(IntPtr hEngine);

        // http://www.solarix.ru/api/ru/sol_ClearError.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_ClearError(IntPtr hEngine);


        // http://www.solarix.ru/api/ru/sol_RestoreCasing.shtml
        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_RestoreCasing(IntPtr hEngine, [In] [Out] StringBuilder Word, int EntryIndex);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_GenerateWordforms(IntPtr hEngine, int EntryID, int npairs, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] int[] pairs);


        // -----------------------------


        [DllImport(gren_dll, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_OpenCorpusStorage8(IntPtr hEngine, string Filename, bool for_writing);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CloseCorpusStorage(IntPtr hEngine, IntPtr hStream);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_WriteSyntaxTree(IntPtr hEngine, IntPtr hStream, string Sentence, IntPtr hTree);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_LoadSyntaxTree(IntPtr hEngine, IntPtr hStream);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_GetTreeHandle(IntPtr hData);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public static extern string sol_GetSentenceW(IntPtr hData);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FreeSyntaxTree(IntPtr hTree);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_CreateLinkages(IntPtr hEngine);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_CreateLinkage(IntPtr hEngine, IntPtr hLinkages);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_AddBeginMarker(IntPtr hEngine, IntPtr hLinkage);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_AddEndMarker(IntPtr hEngine, IntPtr hLinkage);

        [DllImport(gren_dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_AddNodeToLinkage(IntPtr hEngine, IntPtr hLinkage, IntPtr hNode);

        [DllImport(gren_dll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_CreateTreeNodeW(IntPtr hEngine, int id_entry, string word, int n_pair, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] int[] pairs);

        #endregion
    }
}