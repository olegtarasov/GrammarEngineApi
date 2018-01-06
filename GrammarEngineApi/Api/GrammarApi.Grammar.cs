using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GrammarEngineApi.Api
{
    public sealed partial class GrammarApi
    {
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
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_CreateGrammarEngineW(string dictConfigPath);

        /// <summary>
        ///     Создание экземпляра грамматического движка в памяти.
        /// </summary>
        /// <param name="dictConfigPath">Путь к файлу конфигурации словаря (обычно dictionary.xml)</param>
        /// <param name="instanceFlags">Флаги для задания режима работы движка</param>
        /// <returns></returns>
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
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
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteGrammarEngine(IntPtr hEngine);

        // http://www.solarix.ru/api/ru/sol_LoadDictionary.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LoadDictionaryW(IntPtr hEngine, string dictPath);

        // http://www.solarix.ru/api/ru/sol_LoadDictionary.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LoadDictionaryExW(IntPtr hEngine, string dictPath, EngineInstanceFlags flags);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LoadDictionary8(IntPtr hEngine, byte[] DictionaryPath8);

        /// <summary>
        ///     Возвращает компоненты версии скомпилированного движка (через ссылочные аргументы).
        /// </summary>
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetVersion(IntPtr hEngine, ref int majorNumber, ref int minorNumber, ref int buildNumber);

        /// <summary>
        ///     Возвращает номер версии загруженного словаря.
        /// </summary>
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DictionaryVersion(IntPtr hEngine);

        // http://www.solarix.ru/api/ru/sol_CountEntries.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountEntries(IntPtr hEngine);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountForms(IntPtr hEngine);

        // http://www.solarix.ru/api/ru/sol_CountLinks.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountLinks(IntPtr hEngine);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void sol_SetLanguage(IntPtr hEngine, int LanguageID);

        // http://www.solarix.ru/api/ru/sol_MaxLexemLen.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_MaxLexemLen(IntPtr hEngine);

        // http://www.solarix.ru/api/ru/sol_FindEntry.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindEntry(IntPtr hEngine, string name, int partOfSpeechId, int languageId);

        // http://www.solarix.ru/api/ru/sol_FindClass.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindClass(IntPtr hEngine, string partOfSpeechName);

        // http://www.solarix.ru/api/ru/sol_FindEnum.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindEnum(IntPtr hEngine, string attributeName);

        // http://www.solarix.ru/api/ru/sol_FindEnumState.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindEnumState(IntPtr hEngine, int attributeId, string stateName);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_EnumInClass(IntPtr hEngine, int partOfSpeechId, int attributeId);


        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindTagW(IntPtr hEngine, string tagName);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindTagValueW(IntPtr hEngine, int tagID, string valueName);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_FindStringsEx(IntPtr hEngine, string name, int Allow_Dynforms,
                                                      int Synonyms, int Grammar_Links, int Translations, int Semantics, int nJumps);

        // http://www.solarix.ru/api/ru/sol_CountStrings.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountStrings(IntPtr hStr);

        // http://www.solarix.ru/api/ru/sol_GetStringLen.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetStringLen(IntPtr hStr, int i);

        // http://www.solarix.ru/api/ru/sol_GetString.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetStringW(IntPtr hStr, int i, StringBuilder buffer);

        // http://www.solarix.ru/api/ru/sol_DeleteStrings.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteStrings(IntPtr hStr);


        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountNodeMarks(IntPtr hNode);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeMarkNameW(IntPtr hNode, int mark_index, StringBuilder name_buffer);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_SerializeNodeMark(IntPtr hEngine, IntPtr hNode, int mark_index, int format);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetLongStringLenW(IntPtr hString);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetLongStringW(IntPtr hString, StringBuilder buffer);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteLongString(IntPtr hString);


        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Stemmer(IntPtr hEngine, string Word);

        // http://www.solarix.ru/api/ru/sol_GetEntryName.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetEntryName(IntPtr hEngine, int EntryID, StringBuilder Result);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetEntryName8(IntPtr hEngine, int EntryID, byte[] ResultUtf8);

        // http://www.solarix.ru/api/ru/sol_GetEntryClass.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetEntryClass(IntPtr hEngine, int EntryID);

        // http://www.solarix.ru/api/ru/sol_GetEntryCoordState.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetEntryCoordState(IntPtr hEngine, int EntryID, int CategoryID);

        // http://www.solarix.ru/api/ru/sol_FindEntryCoordPair.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindEntryCoordPair(IntPtr hEngine, int EntryID, int CategoryID, int StateID);

        // http://www.solarix.ru/api/ru/sol_GetClassName.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetClassName(IntPtr hEngine, int ClassIndex, StringBuilder Result);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetClassName8(IntPtr hEngine, int ClassIndex, byte[] buffer);


        // http://www.solarix.ru/api/ru/sol_GetCoordType.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetCoordType(IntPtr hEngine, int CoordID, int PartOfSpeechID);

        // http://www.solarix.ru/api/ru/sol_GetCoordName.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetCoordName(IntPtr hEngine, int CoordID, StringBuilder Result);

        // http://www.solarix.ru/api/ru/sol_GetCoordStateName.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetCoordStateName(
            IntPtr hEngine,
            int CoordID,
            int StateID,
            StringBuilder Result
        );

        // http://www.solarix.ru/api/en/sol_CountCoordStates.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountCoordStates(IntPtr h, int CoordID);

        // http://www.solarix.ru/api/en/sol_GetNounGender.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNounGender(IntPtr hStr, int EntryIndex);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNounForm(IntPtr hEngine, int EntryIndex, int Number, int Case, StringBuilder Result);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetVerbForm(IntPtr hEngine, int EntryIndex,
                                                 int Number, int Gender, int Tense, int Person, StringBuilder Result);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetAdjectiveForm(IntPtr hEngine, int EntryIndex,
                                                      int Number, int Gender, int Case, int Anim, int Shortness, int Compar_Form, StringBuilder Result);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CorrNounNumber(IntPtr hEngine, int EntryIndex,
                                                    int Value, int Case, int Anim, StringBuilder Result);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CorrVerbNumber(IntPtr hEngine, int EntryIndex,
                                                    int Value, int Gender, int Tense, StringBuilder Result);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CorrAdjNumber(IntPtr hEngine, int EntryIndex,
                                                   int Value, int Case, int Gender, int Anim, StringBuilder Result);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Syllabs(IntPtr hEngine, string OrgWord, char SyllabDelimiter, StringBuilder Result, int LanguageID);


        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindWord(IntPtr hEngine, string Word,
                                              IntPtr EntryIndex, IntPtr Form, IntPtr Class);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_SeekWord(IntPtr hEngine, string Word, int Allow_Dynforms);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_TranslateToNoun(IntPtr hEngine, int EntryIndex);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_TranslateToInfinitive(IntPtr hEngine, int EntryIndex);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_TranslateToBases(IntPtr hEngine, string Word, int AllowDynforms);

        // http://www.solarix.ru/api/ru/sol_ProjectWord.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_ProjectWord(IntPtr hEngine, string Word, int AllowDynforms);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_ProjectMisspelledWord(IntPtr hEngine, string Word, int AllowDynforms, int nmaxmiss);

        [DllImport(GrenDllName)]
        public static extern IntPtr sol_ProjectMisspelledWord8(IntPtr hEngine, byte[] Word, int AllowDynforms, int nmaxmiss);


        // http://www.solarix.ru/api/ru/sol_CountProjections.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountProjections(IntPtr hList);

        // http://www.solarix.ru/api/ru/sol_DeleteProjections.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void sol_DeleteProjections(IntPtr hList);

        // http://www.solarix.ru/api/ru/sol_GetIEntry.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetIEntry(IntPtr hList, int Index);

        // http://www.solarix.ru/api/ru/sol_GetProjCoordState.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetProjCoordState(IntPtr hEngine, IntPtr hList, int Index, int Coord);


        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetProjCoordCount(IntPtr hEngine, IntPtr hList, int Index);


        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetProjCoordId(IntPtr hEngine, IntPtr hList, int ProjIndex, int TagIndex);


        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetProjStateId(IntPtr hEngine, IntPtr hList, int ProjIndex, int TagIndex);


        // http://www.solarix.ru/api/ru/sol_GetNounGender.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void sol_DeleteResPack(IntPtr hPack);

        // http://www.solarix.ru/api/ru/sol_CountGrafs.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountGrafs(IntPtr hPack);

        // http://www.solarix.ru/api/ru/sol_CountRoots.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountRoots(IntPtr hPack, int iGraf);

        // http://www.solarix.ru/api/ru/sol_GetRoot.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_GetRoot(IntPtr hPack, int iGraf, int iRoot);

        // http://www.solarix.ru/api/ru/sol_CountLeafs.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountLeafs(IntPtr hNode);

        // http://www.solarix.ru/api/ru/sol_GetLeaf.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_GetLeaf(IntPtr hNode, int iLeaf);

        // http://www.solarix.ru/api/ru/sol_GetNodeVersionCount.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVersionCount(IntPtr hEngine, IntPtr hNode);

        // http://www.solarix.ru/api/ru/sol_GetLeafLinkType.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetLeafLinkType(IntPtr hNode, int iLeaf);


        // http://www.solarix.ru/api/ru/sol_GetNodeIEntry.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeIEntry(IntPtr hEngine, IntPtr hNode);

        // http://www.solarix.ru/api/ru/sol_GetNodeVerIEntry.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVerIEntry(IntPtr hEngine, IntPtr hNode, int ialt);

        // http://www.solarix.ru/api/ru/sol_GetNodeContents.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void sol_GetNodeContents(IntPtr hNode, StringBuilder Result);

        // http://www.solarix.ru/api/ru/sol_GetNodePosition.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodePosition(IntPtr hNode);

        // http://www.solarix.ru/api/ru/sol_GetNodeCoordState.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeCoordState(IntPtr hNode, int CoordID);

        // http://www.solarix.ru/api/ru/sol_GetNodeVerCoordState.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVerCoordState(IntPtr hNode, int iver, int CoordID);

        // http://www.solarix.ru/api/ru/sol_GetNodeCoordPair.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeCoordPair(IntPtr hNode, int CoordID, int StateID);

        // http://www.solarix.ru/api/ru/sol_GetNodeVerCoordPair.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVerCoordPair(IntPtr hNode, int iver, int CoordID, int StateID);

        // http://www.solarix.ru/api/ru/sol_GetNodePairsCount.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodePairsCount(IntPtr hNode);

        // http://www.solarix.ru/api/ru/sol_GetNodeVerPairsCount.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVerPairsCount(IntPtr hNode, int iver);

        // http://www.solarix.ru/api/ru/sol_GetNodePairCoord.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodePairCoord(IntPtr hNode, int ipair);

        // http://www.solarix.ru/api/ru/sol_GetNodeVerPairCoord.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVerPairCoord(IntPtr hNode, int iver, int ipair);

        // http://www.solarix.ru/api/ru/sol_GetNodePairState.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodePairState(IntPtr hNode, int ipair);

        // http://www.solarix.ru/api/ru/sol_GetNodeVerPairState.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetNodeVerPairState(IntPtr hNode, int iver, int ipair);

        // http://www.solarix.ru/api/ru/sol_CountInts.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountInts(IntPtr hInts);

        // http://www.solarix.ru/api/ru/sol_GetInt.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetInt(IntPtr hInts, int i);

        // http://www.solarix.ru/api/ru/sol_DeleteInts.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void sol_DeleteInts(IntPtr hInts);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_SeekThesaurus(IntPtr hEngine, int EntryIndex, int Synonyms, int Grammar_Links, int Translation, int Semantics, int nJumps);

        // *********************************************************************************
        // N-grams database functions
        // Overview on Russian: http://www.solarix.ru/for_developers/api/ngrams-api.shtml
        // *********************************************************************************

        // http://www.solarix.ru/api/en/sol_CountNGrams.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountNGrams(IntPtr hEngine, int Type, int Order, out uint Hi, out uint Lo);

        // http://www.solarix.ru/api/ru/sol_Seek1Grams.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Seek1Grams(IntPtr hEngine, int Type, string word1);

        // http://www.solarix.ru/api/ru/sol_Seek2Grams.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Seek2Grams(IntPtr hEngine, int Type, string word1, string word2);

        // http://www.solarix.ru/api/ru/sol_Seek3Grams.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Seek3Grams(IntPtr hEngine, int Type, string word1, string word2, string word3);

        // http://www.solarix.ru/api/ru/sol_Seek4Grams.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Seek4Grams(IntPtr hEngine, int Type, string word1, string word2, string word3, string word4);

        // http://www.solarix.ru/api/ru/sol_Seek5Grams.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Seek5Grams(IntPtr hEngine, int Type, string word1, string word2, string word3, string word4, string word5);

        // --- SentenceBroker API --- 

        // http://www.solarix.ru/api/ru/sol_CreateSentenceBroker.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_CreateSentenceBroker(IntPtr hEngine, string Filename, string DefaultCodepage, int language);

        // http://www.solarix.ru/api/ru/sol_CreateSentenceBrokerMem.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_CreateSentenceBrokerMemW(IntPtr hEngine, string Text, int language);

        // http://www.solarix.ru/api/ru/sol_FetchSentence.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FetchSentence(IntPtr hBroker);

        // http://www.solarix.ru/api/ru/sol_GetFetchedSentenceLen.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetFetchedSentenceLen(IntPtr hBroker);

        // http://www.solarix.ru/api/ru/sol_GetFetchedSentence.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetFetchedSentence(IntPtr hBroker, StringBuilder Buffer);

        // http://www.solarix.ru/api/ru/sol_DeleteSentenceBroker.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void sol_DeleteSentenceBroker(IntPtr hBroker);

        // http://www.solarix.ru/api/ru/sol_Tokenize.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_TokenizeW(IntPtr hEngine, string Text, int language);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Bits();

        // http://www.solarix.ru/api/ru/sol_ListLinksTxt.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_ListLinksTxt(IntPtr hEng, int Key1, int LinkType, int Flags);

        // http://www.solarix.ru/api/ru/sol_DeleteLinksInfo.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteLinksInfo(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList);

        // http://www.solarix.ru/api/ru/sol_LinksInfoCount.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LinksInfoCount(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList);

        // http://www.solarix.ru/api/ru/sol_LinksInfoType.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LinksInfoType(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList, int Index);

        // http://www.solarix.ru/api/ru/sol_LinksInfoID.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LinksInfoID(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList, int Index);

        // http://www.solarix.ru/api/ru/sol_LinksInfoEKey1.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LinksInfoEKey1(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList, int Index);

        // http://www.solarix.ru/api/ru/sol_LinksInfoEKey2.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_LinksInfoEKey2(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList, int Index);

        // http://www.solarix.ru/api/ru/sol_LinksInfoTagsTxt.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern /*wchar_t**/ IntPtr sol_LinksInfoTagsTxt(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList, int Index);

        // http://www.solarix.ru/api/ru/sol_LinksInfoFlagsTxt.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern /*wchar_t**/ IntPtr sol_LinksInfoFlagsTxt(IntPtr hEng, IntPtr /*HLINKSINFO*/ hList, int Index);

        // http://www.solarix.ru/api/ru/sol_DeleteLink.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteLink(IntPtr hEng, int LinkID, int LinkType);

        // http://www.solarix.ru/api/ru/sol_AddLink.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_AddLink(IntPtr hEng, int LinkType, int IE1, int LinkCode, int IE2, string Tags);


        // http://www.solarix.ru/api/ru/sol_FindLanguage.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindLanguage(IntPtr hEng, string LanguageName);

        /// <summary>
        ///     Find a phrase given a phrase text.
        ///     <para>http://www.solarix.ru/api/en/sol_FindPhrase.shtml</para>
        /// </summary>
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FindPhrase(IntPtr hEng, string Phrase, int Flags);

        /// <summary>
        ///     Create new phrase entry in lexicon.
        ///     <para>http://www.solarix.ru/api/en/sol_AddPhrase.shtml</para>
        /// </summary>
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_AddPhrase(IntPtr hEng, string Phrase, int LanguageID, int ClassID, int ProcessingFlags);

        /// <summary>
        ///     Retrieve the text contents of the phrase.
        ///     <para>http://www.solarix.ru/api/en/sol_GetPhraseText.shtml</para>
        /// </summary>
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern /*wchar_t**/ IntPtr sol_GetPhraseText(IntPtr hEng, int PhraseId);

        /// <summary>
        ///     Get the language ID if it was set for the phrase
        ///     <para>http://www.solarix.ru/api/en/sol_GetPhraseLanguage.shtml</para>
        /// </summary>
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetPhraseLanguage(IntPtr hEng, int PhraseId);

        /// <summary>
        ///     Get the part of speech ID if it was set for the phrase
        ///     <para>http://www.solarix.ru/api/en/sol_GetPhraseClass.shtml</para>
        /// </summary>
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetPhraseClass(IntPtr hEng, int PhraseId);


        // http://www.solarix.ru/api/ru/sol_Free.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_Free(IntPtr hEngine, IntPtr ptr);


        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern /* wchar_t* */ IntPtr sol_NormalizePhraseW(IntPtr hEng, IntPtr hLinkages);

        // http://www.solarix.ru/api/ru/sol_AddWord.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_AddWord(IntPtr hEng, string Txt);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_ProcessPhraseEntry(
            IntPtr hEngine,
            int PhraseId,
            string Scenario,
            int Language,
            char DelimiterChar
        );

        // http://www.solarix.ru/api/ru/sol_DeletePhrase.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeletePhrase(IntPtr hEngine, int PhraseId);


        // http://www.solarix.ru/api/ru/sol_ListEntries.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr /*IntPtr_INTARRAY*/ sol_ListEntries(IntPtr hEngine, int Flags, int EntryType, string Mask, int Language, int PartOfSpeech);


        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_ListEntryForms(IntPtr hEngine, int EntryKey);


        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
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
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
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
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_SyntaxAnalysis(
            IntPtr hEngine,
            string sentence,
            MorphologyFlags flags,
            SyntaxFlags unusedFlags,
            int constraints,
            int languageID
        );


        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr /*HFLEXIONS*/ sol_FindFlexionHandlers(IntPtr hEnging, string WordBasicForm, int SearchEntries);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteFlexionHandlers(IntPtr hEngine, IntPtr /*HFLEXIONS*/ hFlexs);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountEntriesInFlexionHandlers(IntPtr hEngine, IntPtr /*HFLEXIONS*/ hFlexs);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CountParadigmasInFlexionHandlers(IntPtr hEngine, IntPtr /*HFLEXIONS*/ hFlexs);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetEntryInFlexionHandlers(IntPtr hEngine, IntPtr /*HFLEXIONS*/ hFlexs, int Index);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetParadigmaInFlexionHandlers(IntPtr hEngine, IntPtr /*HFLEXIONS*/ hFlexs, int Index, StringBuilder ParadigmaName);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr /*HFLEXIONTABLE*/ sol_BuildFlexionHandler(
            IntPtr hEngine,
            IntPtr /*HFLEXIONS*/ hFlexs,
            string ParadigmaName,
            int EntryIndex
        );

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr /*const wchar_t* */ sol_GetFlexionHandlerWordform(
            IntPtr hEngine,
            IntPtr /*HFLEXIONTABLE*/ hFlex,
            string dims
        );


        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_DeleteFlexionHandler(IntPtr hEngine, IntPtr /*HFLEXIONTABLE*/ hFlex);

        // http://www.solarix.ru/api/ru/sol_SetLinkFlags.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_SetLinkFlags(IntPtr hEngine, int id_link, string Flags);

        // http://www.solarix.ru/api/ru/sol_SetLinkTags.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_SetLinkTags(IntPtr hEngine, int LinkType, int id_link, string Tags);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_SetPhraseNote(IntPtr hEngine, int te_id, string Name, string Value);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_ReserveLexiconSpace(IntPtr hEngine, int n);

        // http://www.solarix.ru/api/ru/sol_GetError.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetError(IntPtr hEngine, StringBuilder buffer, int buffer_len);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetError8(IntPtr hEngine, byte[] buffer, int buffer_len);

        // http://www.solarix.ru/api/ru/sol_GetErrorLen.shtml
        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetErrorLen(IntPtr hEngine);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_GetErrorLen8(IntPtr hEngine);

        // http://www.solarix.ru/api/ru/sol_ClearError.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_ClearError(IntPtr hEngine);


        // http://www.solarix.ru/api/ru/sol_RestoreCasing.shtml
        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_RestoreCasing(IntPtr hEngine, [In] [Out] StringBuilder Word, int EntryIndex);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_GenerateWordforms(IntPtr hEngine, int EntryID, int npairs, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] int[] pairs);


        // -----------------------------


        [DllImport(GrenDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_OpenCorpusStorage8(IntPtr hEngine, string Filename, bool for_writing);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_CloseCorpusStorage(IntPtr hEngine, IntPtr hStream);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_WriteSyntaxTree(IntPtr hEngine, IntPtr hStream, string Sentence, IntPtr hTree);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_LoadSyntaxTree(IntPtr hEngine, IntPtr hStream);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_GetTreeHandle(IntPtr hData);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public static extern string sol_GetSentenceW(IntPtr hData);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_FreeSyntaxTree(IntPtr hTree);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_CreateLinkages(IntPtr hEngine);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_CreateLinkage(IntPtr hEngine, IntPtr hLinkages);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_AddBeginMarker(IntPtr hEngine, IntPtr hLinkage);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_AddEndMarker(IntPtr hEngine, IntPtr hLinkage);

        [DllImport(GrenDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int sol_AddNodeToLinkage(IntPtr hEngine, IntPtr hLinkage, IntPtr hNode);

        [DllImport(GrenDllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr sol_CreateTreeNodeW(IntPtr hEngine, int id_entry, string word, int n_pair, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] int[] pairs);
    }
}