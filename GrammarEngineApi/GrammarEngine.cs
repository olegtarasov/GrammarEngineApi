using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using GrammarEngineApi.Properties;

namespace GrammarEngineApi
{
    public class GrammarEngine : IDisposable
    {
        private readonly IntPtr _engine;

        public GrammarEngine()
        {
            UnpackResources();
            _engine = GrammarApi.sol_CreateGrammarEngineW(null);
        }

        public GrammarEngine(IntPtr engine)
        {
            UnpackResources();
            _engine = engine;
            Initialized = true;
        }

        public GrammarEngine(string dictionaryPath)
        {
            UnpackResources();
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
            var result = LinuxHandler(() => GrammarApi.sol_LoadDictionary8(_engine, GetUtf8Bytes(dictionaryPath)),
                () => GrammarApi.sol_LoadDictionaryW(_engine, dictionaryPath));

            if (result != 1)
            {
                var err = GetLastError();
                throw new InvalidOperationException($"Failed to load dictionary from {dictionaryPath}. {err}");
            }

            Initialized = true;
        }

        public AnalysisResults AnalyzeMorphology(string phrase, Languages language)
        {
            var hPack = GrammarApi.sol_MorphologyAnalysis(_engine, phrase, 0, 0, 0, (int)language);
            var res = new AnalysisResults(this, hPack);
            return res;
        }

        public AnalysisResults AnalyzeMorphology(string phrase, Languages language, MorphologyFlags flags)
        {
            var hPack = GrammarApi.sol_MorphologyAnalysis(_engine, phrase, flags, 0, 0, (int)language);
            var res = new AnalysisResults(this, hPack);
            return res;
        }

        public AnalysisResults AnalyzeMorphology(string phrase, Languages language, MorphologyFlags flags, int constraints)
        {
            var hPack = GrammarApi.sol_MorphologyAnalysis(_engine, phrase, flags, 0, constraints, (int)language);
            var res = new AnalysisResults(this, hPack);
            return res;
        }


        public AnalysisResults AnalyzeSyntax(string phrase, Languages language)
        {
            var hPack = GrammarApi.sol_SyntaxAnalysis(_engine, phrase, 0, 0, 60000, (int)language);
            var res = new AnalysisResults(this, hPack);
            return res;
        }

        public AnalysisResults AnalyzeSyntax(string phrase, Languages language, MorphologyFlags morphFlags, SyntaxFlags syntaxFlags)
        {
            var hPack = GrammarApi.sol_SyntaxAnalysis(_engine, phrase, morphFlags, syntaxFlags, 60000, (int)language);
            var res = new AnalysisResults(this, hPack);
            return res;
        }

        public AnalysisResults AnalyzeSyntax(string phrase, Languages language, MorphologyFlags morphFlags, SyntaxFlags syntaxFlags, int constraints)
        {
            var hPack = GrammarApi.sol_SyntaxAnalysis(_engine, phrase, morphFlags, syntaxFlags, constraints, (int)language);
            var res = new AnalysisResults(this, hPack);
            return res;
        }

        /// <summary>
        ///     Split the string into words and return the list of these words.
        ///     Language-specific rules are used to process dots, hyphens etc.
        /// </summary>
        public string[] Tokenize(string text, Languages language)
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


        public int CountCoordStates(int coordId)
        {
            return GrammarApi.sol_CountCoordStates(GetEngineHandle(), coordId);
        }

        public int CountWordEntries()
        {
            return GrammarApi.sol_CountEntries(_engine);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int FindCoord(string coordName)
        {
            return GrammarApi.sol_FindEnum(_engine, coordName);
        }


        public int FindEntry(string entryName, int partOfSpeech)
        {
            return GrammarApi.sol_FindEntry(_engine, entryName, partOfSpeech, -1);
        }

        public int FindPartOfSpeech(string partOfSpeechName)
        {
            return GrammarApi.sol_FindClass(_engine, partOfSpeechName);
        }

        public int FindPhrase(string phraseText, bool caseSensitive)
        {
            return GrammarApi.sol_FindPhrase(_engine, phraseText, caseSensitive ? 1 : 0);
        }

        public int FindState(int coordId, string stateName)
        {
            return GrammarApi.sol_FindEnumState(_engine, coordId, stateName);
        }

        public int FindTag(string tagName)
        {
            return GrammarApi.sol_FindTagW(GetEngineHandle(), tagName);
        }

        public int FindTagValue(int tagId, string valueName)
        {
            return GrammarApi.sol_FindTagValueW(GetEngineHandle(), tagId, valueName);
        }

        public WordProjections FindWordForm(string wordform)
        {
            return new WordProjections(_engine, GrammarApi.sol_ProjectWord(_engine, wordform, 0));
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

        public string GetCoordName(int coordId)
        {
            var b = new StringBuilder(32);
            GrammarApi.sol_GetCoordName(_engine, coordId, b);
            return b.ToString();
        }

        public string GetCoordStateName(int coordId, int stateId)
        {
            var b = new StringBuilder(32);
            GrammarApi.sol_GetCoordStateName(_engine, coordId, stateId, b);
            return b.ToString();
        }

        public int GetCoordType(int partOfSpeechId, int coordId)
        {
            return GrammarApi.sol_GetCoordType(GetEngineHandle(), coordId, partOfSpeechId);
        }

        public IntPtr GetEngineHandle()
        {
            return _engine;
        }


        public int GetEntryAttrState(int entryId, int coordId)
        {
            return GrammarApi.sol_GetEntryCoordState(_engine, entryId, coordId);
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

        public List<string> SplitSentenses(string input)
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

        public string GetPhraseText(int phraseId)
        {
            return GrammarApi.sol_GetPhraseTextFX(_engine, phraseId);
        }

        public TextSegmenter GetTextFileSegmenter(string filePath, string encoding, int languageId)
        {
            var h = GrammarApi.sol_CreateSentenceBroker(GetEngineHandle(), filePath, encoding, languageId);
            return new TextSegmenter(this, h);
        }


        public ThesaurusLinks ListLinksTxt(int idEntry, int linkCode, int flags)
        {
            var hList = GrammarApi.sol_ListLinksTxt(GetEngineHandle(), idEntry, linkCode, flags);
            return new ThesaurusLinks(GetEngineHandle(), hList);
        }

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

        protected virtual void Dispose(bool disposing)
        {
            if (_engine != IntPtr.Zero)
            {
                GrammarApi.sol_DeleteGrammarEngine(_engine);
            }
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

        private void UnpackResources()
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

            UnpackFile(curDir, "libdescr.dll", Resources.libdesr);
            UnpackFile(curDir, "sqlite.dll", Resources.sqlite);
            UnpackFile(curDir, "solarix_grammar_engine.dll", Resources.solarix_grammar_engine);
        }

        private void UnpackFile(string curDir, string fileName, byte[] bytes)
        {
            var path = !string.IsNullOrEmpty(curDir) ? Path.Combine(curDir, fileName) : fileName;
            if (File.Exists(path))
            {
                return;
            }

            File.WriteAllBytes(path, bytes);
        }

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