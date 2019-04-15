using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using GrammarEngineApi.Api;
using GrammarEngineApi.Logging;
using GrammarEngineApi.Properties;
using NativeLibraryManager;

namespace GrammarEngineApi
{
    /// <summary>
    ///     Main Grammar Engine and Lemmatizer API.
    /// </summary>
    public class GrammarEngine : IDisposable
    {
        private static readonly ILog _log = LogProvider.For<GrammarEngine>();
        private static readonly LibraryManager _libraryManager;

        private readonly IntPtr _engine;

        //private IntPtr _lemmatizer;

        static GrammarEngine()
        {
            var accessor = new ResourceAccessor();
            _libraryManager = new LibraryManager(
                Assembly.GetExecutingAssembly(),
                new LibraryItem(Platform.MacOs, Bitness.x64,
                    new LibraryFile(ResourceNamesWindows.Sqlite3, accessor.Binary(ResourceNamesWindows.Resource(ResourceNamesWindows.Sqlite3))),
                    new LibraryFile(ResourceNamesWindows.BoostDateTime, accessor.Binary(ResourceNamesWindows.Resource(ResourceNamesWindows.BoostDateTime))),
                    new LibraryFile(ResourceNamesWindows.BoostRegex, accessor.Binary(ResourceNamesWindows.Resource(ResourceNamesWindows.BoostRegex))),
                    new LibraryFile(ResourceNamesWindows.BoostSystem, accessor.Binary(ResourceNamesWindows.Resource(ResourceNamesWindows.BoostSystem))),
                    new LibraryFile(ResourceNamesWindows.GrammarEngine, accessor.Binary(ResourceNamesWindows.Resource(ResourceNamesWindows.GrammarEngine)))
                )
            );
        }

        public GrammarEngine()
        {
            _libraryManager.LoadNativeLibrary();
            _engine = GrammarApi.sol_CreateGrammarEngineW(null);
        }

        public GrammarEngine(IntPtr engine)
        {
            _libraryManager.LoadNativeLibrary();
            _engine = engine;
            Initialized = true;
        }

        public GrammarEngine(string dictionaryPath)
        {
            _libraryManager.LoadNativeLibrary();
            _engine = GrammarApi.sol_CreateGrammarEngineW(null);
            LoadDictionary(dictionaryPath);
        }

#if NETSTANDARD || NETCORE || NETSTANDARD2_0 // должны быть определены в проекте через <DefineConstants>...</DefineConstants>
// https://github.com/dotnet/corefx/blob/master/src/System.Runtime.InteropServices.RuntimeInformation/ref/System.Runtime.InteropServices.RuntimeInformation.cs
        public bool IsLinux { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#else
        public bool IsLinux { get; } = (int)Environment.OSVersion.Platform == 4 || (int)Environment.OSVersion.Platform == 6 || (int)Environment.OSVersion.Platform == 128;
#endif

        public bool Initialized { get; private set; }

        public void LoadDictionary(string dictionaryPath)
        {
            _log.Info($"Loading dictionary from {dictionaryPath}");

            string dir = Path.GetDirectoryName(dictionaryPath);
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            {
                WarnAndThrow("Dictionary directory not found!");
            }

            string dicPath = Path.Combine(dir, "dictionary.xml");
            if (!File.Exists(dicPath))
            {
                WarnAndThrow("Dictionary file not found!");
            }

            //string lemPath = Path.Combine(dir, "lemmatizer.db");
            //if (!File.Exists(lemPath))
            //{
            //    throw new InvalidOperationException("Lemmatizer database not found!");
            //}

            var result = LinuxHandler(() => GrammarApi.sol_LoadDictionary8(_engine, GetUtf8Bytes(dictionaryPath)),
                () => GrammarApi.sol_LoadDictionaryW(_engine, dictionaryPath));

            if (result != 1)
            {
                var err = GetLastError();
                WarnAndThrow($"Failed to load dictionary from {dicPath}. {err}");
            }

            //_lemmatizer = GrammarApi.sol_LoadLemmatizatorW(lemPath, LemmatizerFlags.Default);
            //if (_lemmatizer == IntPtr.Zero)
            //{
            //    var err = GetLastError();
            //    throw new InvalidOperationException($"Failed to load dictionary from {dicPath}. {err}");
            //}

            Initialized = true;

            _log.Info("Loaded dictionary.");
        }

        private void WarnAndThrow(string message)
        {
            _log.Warn(message);
            throw new InvalidOperationException(message);
        }

        #region Syntax and morphology

        public AnalysisResults AnalyzeMorphology(string phrase, Languages language)
        {
            return AnalyzeMorphology(phrase, language, MorphologyFlags.DEFAULT, 0);
        }

        public AnalysisResults AnalyzeMorphology(string phrase, Languages language, MorphologyFlags flags)
        {
            return AnalyzeMorphology(phrase, language, flags, 0);
        }

        public AnalysisResults AnalyzeMorphology(string phrase, Languages language, MorphologyFlags flags, int constraints)
        {
            var hPack = GrammarApi.sol_MorphologyAnalysis(_engine, phrase, flags, 0, constraints, (int)language);
            var res = new AnalysisResults(this, hPack);
            return res;
        }


        public AnalysisResults AnalyzeSyntax(string phrase, Languages language)
        {
            return AnalyzeSyntax(phrase, language, MorphologyFlags.DEFAULT, SyntaxFlags.DEFAULT, 0);
        }

        public AnalysisResults AnalyzeSyntax(string phrase, Languages language, MorphologyFlags morphFlags, SyntaxFlags syntaxFlags)
        {
            return AnalyzeSyntax(phrase, language, morphFlags, syntaxFlags, 0);
        }

        public AnalysisResults AnalyzeSyntax(string phrase, Languages language, MorphologyFlags morphFlags, SyntaxFlags syntaxFlags, int constraints)
        {
            var hPack = GrammarApi.sol_SyntaxAnalysis(_engine, phrase, morphFlags, syntaxFlags, constraints, (int)language);
            var res = new AnalysisResults(this, hPack);
            return res;
        }

        #endregion

        #region Segmentation

        /// <summary>
        ///     Split the string into words and return the list of these words.
        ///     Language-specific rules are used to process dots, hyphens etc.
        /// </summary>
        /// <remarks>
        /// Works only on pre-segmented sentnces.
        /// </remarks>
        public string[] TokenizeSentence(string text, Languages language)
        {
            var hTokens = GrammarApi.sol_TokenizeW(_engine, text, (int)language);

            string[] tokens = null;
            var maxWordLen = GrammarApi.sol_MaxLexemLen(_engine) + 1;

            if (hTokens != (IntPtr)null)
            {
                var ntoken = GrammarApi.sol_CountStrings(hTokens);
                tokens = new string[ntoken];

                var buffer = new StringBuilder(maxWordLen);
                for (var i = 0; i < ntoken; ++i)
                {
                    buffer.Length = 0;
                    GrammarApi.sol_GetStringW(hTokens, i, buffer);
                    tokens[i] = buffer.ToString();
                }

                GrammarApi.sol_DeleteStrings(hTokens);
            }

            return tokens;
        }

        /// <summary>
        ///     Split the string into words and return the list of these words
        ///     in a single string separated with specified character.
        ///     Language-specific rules are used to process dots, hyphens etc.
        /// </summary>
        public string TokenizeWithSeparator(string text, Languages language, char separator = '|')
        {
            var hTokens = GrammarApi.sol_TokenizeW(_engine, text, (int)language);
            if (hTokens == IntPtr.Zero)
            {
                return string.Empty;
            }

            var result = new StringBuilder(text.Length);
            int maxWordLen = GrammarApi.sol_MaxLexemLen(_engine) + 1;
            int ntoken = GrammarApi.sol_CountStrings(hTokens);
            
            var buffer = new StringBuilder(maxWordLen);
            for (var i = 0; i < ntoken; ++i)
            {
                buffer.Length = 0;
                GrammarApi.sol_GetStringW(hTokens, i, buffer);
                result.Append(buffer.ToString()).Append(separator);
            }

            result.Length--;
            GrammarApi.sol_DeleteStrings(hTokens);

            return result.ToString();
        }

        public List<string> SplitSentences(string input)
        {
            var broker = GrammarApi.sol_CreateSentenceBrokerMemW(_engine, input, (int)Languages.RUSSIAN_LANGUAGE);
            var result = new List<string>();

            int len;
            while ((len = GrammarApi.sol_FetchSentence(broker)) >= 0)
            {
                if (len > 0)
                {
                    var b = new StringBuilder(len + 2);
                    GrammarApi.sol_GetFetchedSentence(broker, b);
                    result.Add(b.ToString());
                }
            }

            GrammarApi.sol_DeleteSentenceBroker(broker);

            return result;
        }

        /// <summary>
        /// Creates a segmenter that reads sentences from a text file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="isUtf8">Indicates whether file has UTF-8 encoding. Unicode assumed otherwise.</param>
        /// <param name="language">Language to use.</param>
        /// <returns>Text file segmenter.</returns>
        public TextFileSegmenter CreateTextFileSegmenter(string filePath, bool isUtf8, Languages language)
        {
            var h = GrammarApi.sol_CreateSentenceBroker(GetEngineHandle(), filePath, isUtf8 ? "utf-8" : "unicode", (int)language);
            if (h == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create the segmenter!");
            }

            return new TextFileSegmenter(h);
        }

        #endregion

        #region Entries

        public Entry GetEntry(int id)
        {
            return new Entry(id, GetEntryName(id), (WordClassesRu)GetEntryClass(id));
        }

        public int FindEntry(string entryName, int partOfSpeech)
        {
            return GrammarApi.sol_FindEntry(_engine, entryName, partOfSpeech, -1);
        }

        public int FindPhrase(string phraseText, bool caseSensitive)
        {
            return GrammarApi.sol_FindPhrase(_engine, phraseText, caseSensitive ? 1 : 0);
        }

        public int CountWordEntries()
        {
            return GrammarApi.sol_CountEntries(_engine);
        }

        public int GetEntryClass(int idEntry)
        {
            return GrammarApi.sol_GetEntryClass(_engine, idEntry);
        }

        public string GetEntryName(int idEntry)
        {
            if (IsLinux)
            {
                var buf8 = GetLexemBuffer8();
                GrammarApi.sol_GetEntryName8(_engine, idEntry, buf8);
                return Utf8ToString(buf8);
            }

            var b = new StringBuilder(32); // магическая константа 32 - фактически сейчас слов длиннее 32 символов в словарях нет.
            GrammarApi.sol_GetEntryName(_engine, idEntry, b);
            return b.ToString();
        }

        public int GetEntryAttrState(int entryId, int coordId)
        {
            return GrammarApi.sol_GetEntryCoordState(_engine, entryId, coordId);
        }

        public string GetPhraseText(int phraseId)
        {
            return GrammarApi.sol_GetPhraseTextFX(_engine, phraseId);
        }

        #endregion

        #region Coordinates and states

        public int CountCoordStates(int coordId)
        {
            return GrammarApi.sol_CountCoordStates(GetEngineHandle(), coordId);
        }

        public int FindCoord(string coordName)
        {
            return GrammarApi.sol_FindEnum(_engine, coordName);
        }

        public int FindState(int coordId, string stateName)
        {
            return GrammarApi.sol_FindEnumState(_engine, coordId, stateName);
        }

        public string GetCoordStateName(int coordId, int stateId)
        {
            return GrammarApi.GetCString(b => GrammarApi.sol_GetCoordStateName(_engine, coordId, stateId, b));
        }

        public string GetCoordName(int coordId)
        {
            return GrammarApi.GetCString(b => GrammarApi.sol_GetCoordName(_engine, coordId, b));
        }

        public int GetCoordType(int partOfSpeechId, int coordId)
        {
            return GrammarApi.sol_GetCoordType(GetEngineHandle(), coordId, partOfSpeechId);
        }

        #endregion


        #region Dictionaries (classes, tags etc)

        public string GetClassName(int partOfSpeechId)
        {
            if (IsLinux)
            {
                var buf8 = GetLexemBuffer8();
                GrammarApi.sol_GetClassName8(_engine, partOfSpeechId, buf8);
                return Utf8ToString(buf8);
            }

            var b = new StringBuilder(32);
            GrammarApi.sol_GetClassName(_engine, partOfSpeechId, b);
            return b.ToString();
        }

        public int FindPartOfSpeech(string partOfSpeechName)
        {
            return GrammarApi.sol_FindClass(_engine, partOfSpeechName);
        }

        public int FindTag(string tagName)
        {
            return GrammarApi.sol_FindTagW(GetEngineHandle(), tagName);
        }

        public int FindTagValue(int tagId, string valueName)
        {
            return GrammarApi.sol_FindTagValueW(GetEngineHandle(), tagId, valueName);
        }

        #endregion

        #region Word forms and tesaurus

        public ProjectionResults FindWordForms(string wordform)
        {
            return new ProjectionResults(this, GrammarApi.sol_ProjectWord(_engine, wordform, 0));
        }

        public ProjectionResults ProjectMisspelledWord(string word, int nmaxmiss)
        {
            return new ProjectionResults(this, GrammarApi.sol_ProjectMisspelledWord(_engine, word, 0, nmaxmiss));
        }
        public List<string> GenerateWordforms(int entryId, List<int> coordId, List<int> stateId)
        {
            var npairs = coordId.Count;
            var pairs = new int[npairs * 2];
            for (int i = 0, j = 0; i < npairs; ++i)
            {
                pairs[j++] = coordId[i];
                pairs[j++] = stateId[i];
            }

            var res = new List<string>();
            var hStr = GrammarApi.sol_GenerateWordforms(_engine, entryId, npairs, pairs);
            if (hStr != (IntPtr)0)
            {
                var nstr = GrammarApi.sol_CountStrings(hStr);
                for (var k = 0; k < nstr; ++k)
                {
                    res.Add(GrammarApi.sol_GetStringFX(hStr, k));
                }

                GrammarApi.sol_DeleteStrings(hStr);
            }

            return res;
        }

        public List<int> GetLinks(int idEntry, int linkType)
        {
            var res = new List<int>();

            var hList = GrammarApi.sol_ListLinksTxt(_engine, idEntry, linkType, 0);
            if (hList != IntPtr.Zero)
            {
                var n = GrammarApi.sol_LinksInfoCount(_engine, hList);
                for (var i = 0; i < n; ++i)
                {
                    var idEntry2 = GrammarApi.sol_LinksInfoEKey2(_engine, hList, i);
                    res.Add(idEntry2);
                }

                GrammarApi.sol_DeleteLinksInfo(_engine, hList);
            }

            return res;
        }


        public string GetNounForm(int id, int number, int @case)
        {
            var sb = new StringBuilder();
            GrammarApi.sol_GetNounForm(GetEngineHandle(), id, number, @case, sb);
            return sb.ToString();
        }

        public string GetVerbForm(int id, int number, int gender, int tense, int person)
        {
            var sb = new StringBuilder();
            GrammarApi.sol_GetVerbForm(GetEngineHandle(), id, number, gender, tense, person, sb);
            return sb.ToString();
        }

        public int TranslateToInfinitive(int id)
        {
            return GrammarApi.sol_TranslateToInfinitive(GetEngineHandle(), id);
        }

        public List<int> GetPhrasalLinks(int idPhrase, int linkType)
        {
            var res = new List<int>();

            var hList = GrammarApi.sol_ListLinksTxt(_engine, idPhrase, linkType, 1);
            if (hList != IntPtr.Zero)
            {
                var n = GrammarApi.sol_LinksInfoCount(_engine, hList);
                for (var i = 0; i < n; ++i)
                {
                    var idPhrase2 = GrammarApi.sol_LinksInfoEKey2(_engine, hList, i);
                    res.Add(idPhrase2);
                }

                GrammarApi.sol_DeleteLinksInfo(_engine, hList);
            }

            return res;
        }


        public ThesaurusLinks ListLinksTxt(int idEntry, int linkCode, int flags)
        {
            var hList = GrammarApi.sol_ListLinksTxt(GetEngineHandle(), idEntry, linkCode, flags);
            return new ThesaurusLinks(GetEngineHandle(), hList);
        }

        #endregion

        //#region Lemmatization

        ///// <summary>
        ///// Lemmatize sentence. By default expects tokens to be separated by '|'.
        ///// </summary>
        ///// <param name="sentence">Sentence to lemmatize.</param>
        ///// <param name="separator">Token separator.</param>
        ///// <returns>Lemmatized tokens.</returns>
        //public string[] LemmatizeSentence(string sentence, char separator = '|')
        //{
        //    if (string.IsNullOrEmpty(sentence))
        //    {
        //        return new string[0];
        //    }

        //    var lemResult = GrammarApi.sol_LemmatizePhraseW(_lemmatizer, sentence, 0, separator);
        //    if (lemResult == IntPtr.Zero)
        //    {
        //        return new string[0];
        //    }

        //    int lemmaCnt = GrammarApi.sol_CountLemmas(lemResult);
        //    var result = new string[lemmaCnt];
        //    var buffer = new StringBuilder(120);
        //    for (int i = 0; i < lemmaCnt; i++)
        //    {
        //        GrammarApi.sol_GetLemmaStringW(lemResult, i, buffer, 120);
        //        result[i] = buffer.ToString();
        //        buffer.Clear();
        //    }

        //    GrammarApi.sol_DeleteLemmas(lemResult);

        //    return result;
        //}

        //#endregion

        #region Misc

        public string NormalizePhrase(AnalysisResults linkages)
        {
            var wchar_ptr = GrammarApi.sol_NormalizePhraseW(_engine, linkages.GetHandle());

            if (wchar_ptr == (IntPtr)null)
            {
                return "";
            }

            var res = Marshal.PtrToStringUni(wchar_ptr);
            GrammarApi.sol_Free(_engine, wchar_ptr);
            return res;
        }


        public string RestoreCasing(int entryId, string word)
        {
            return GrammarApi.sol_RestoreCasingFX(_engine, word, entryId);
        }

        #endregion

        public IntPtr GetEngineHandle()
        {
            return _engine;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_engine != IntPtr.Zero)
            {
                GrammarApi.sol_DeleteGrammarEngine(_engine);
            }

            //if (_lemmatizer != IntPtr.Zero)
            //{
            //    GrammarApi.sol_DeleteLemmatizator(_lemmatizer);
            //}
        }

        private T LinuxHandler<T>(Func<T> isLinux, Func<T> isOther)
        {
            if (IsLinux)
            {
                return isLinux();
            }

            return isOther();
        }

        private byte[] GetUtf8Bytes(string input) => Encoding.UTF8.GetBytes(input);

        private static byte[] GetLexemBuffer8()
        {
            return new byte[32 * 6];
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

        public string GetLastError()
        {
            if (IsLinux)
            {
                var len = GrammarApi.sol_GetErrorLen8(_engine);
                if (len == 0)
                {
                    return "";
                }

                var errUtf8 = new byte[len];
                GrammarApi.sol_GetError8(_engine, errUtf8, len);

                GrammarApi.sol_ClearError(_engine);
                return Encoding.UTF8.GetString(errUtf8);
            }
            else
            {
                var len = GrammarApi.sol_GetErrorLen(_engine);
                if (len == 0)
                {
                    return "";
                }

                var b = new StringBuilder(len + 1);
                GrammarApi.sol_GetError(_engine, b, len);
                GrammarApi.sol_ClearError(_engine);
                return b.ToString();
            }
        }
    }
}