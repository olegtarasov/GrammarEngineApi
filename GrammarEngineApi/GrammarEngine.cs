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

        public GrammarEngine(IntPtr engine)
        {
            UnpackResources();
            _engine = engine;
        }

        public GrammarEngine(string dictionaryPath)
        {
            UnpackResources();
            _engine = GrammarEngineApi.sol_CreateGrammarEngineW(null);
            int result = LinuxHandler(() => GrammarEngineApi.sol_LoadDictionary8(_engine, GetUtf8Bytes(dictionaryPath)),
                () => GrammarEngineApi.sol_LoadDictionaryW(_engine, dictionaryPath));

            if (result != 1)
            {
                string err = GrammarEngineApi.sol_GetErrorFX(_engine);
                throw new InvalidOperationException($"Failed to load dictionary from {dictionaryPath}. {err}");
            }
        }

#if NETSTANDARD || NETCORE || NETSTANDARD2_0 // должны быть определены в проекте через <DefineConstants>...</DefineConstants>
        // https://github.com/dotnet/corefx/blob/master/src/System.Runtime.InteropServices.RuntimeInformation/ref/System.Runtime.InteropServices.RuntimeInformation.cs
        public bool IsLinux { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#else
        public bool IsLinux { get; } = (int)Environment.OSVersion.Platform == 4 || (int)Environment.OSVersion.Platform == 6 || (int)Environment.OSVersion.Platform == 128;
#endif

        public AnalysisResults AnalyzeMorphology(string phrase, Languages language)
        {
            IntPtr hPack = GrammarEngineApi.sol_MorphologyAnalysis(_engine, phrase, 0, 0, 0, (int)language);
            AnalysisResults res = new AnalysisResults(this, hPack);
            return res;
        }

        public AnalysisResults AnalyzeMorphology(string phrase, Languages language, MorphologyFlags flags)
        {
            IntPtr hPack = GrammarEngineApi.sol_MorphologyAnalysis(_engine, phrase, flags, 0, 0, (int)language);
            AnalysisResults res = new AnalysisResults(this, hPack);
            return res;
        }

        public AnalysisResults AnalyzeMorphology(string phrase, Languages language, MorphologyFlags flags, int constraints)
        {
            IntPtr hPack = GrammarEngineApi.sol_MorphologyAnalysis(_engine, phrase, flags, 0, constraints, (int)language);
            AnalysisResults res = new AnalysisResults(this, hPack);
            return res;
        }


        public AnalysisResults AnalyzeSyntax(string phrase, Languages language)
        {
            IntPtr hPack = GrammarEngineApi.sol_SyntaxAnalysis(_engine, phrase, 0, 0, 60000, (int)language);
            AnalysisResults res = new AnalysisResults(this, hPack);
            return res;
        }

        public AnalysisResults AnalyzeSyntax(string phrase, Languages language, MorphologyFlags morphFlags, SyntaxFlags syntaxFlags)
        {
            IntPtr hPack = GrammarEngineApi.sol_SyntaxAnalysis(_engine, phrase, morphFlags, syntaxFlags, 60000, (int)language);
            AnalysisResults res = new AnalysisResults(this, hPack);
            return res;
        }

        public AnalysisResults AnalyzeSyntax(string phrase, Languages language, MorphologyFlags morphFlags, SyntaxFlags syntaxFlags, int constraints)
        {
            IntPtr hPack = GrammarEngineApi.sol_SyntaxAnalysis(_engine, phrase, morphFlags, syntaxFlags, constraints, (int)language);
            AnalysisResults res = new AnalysisResults(this, hPack);
            return res;
        }

        public int CountCoordStates(int coordId)
        {
            return GrammarEngineApi.sol_CountCoordStates(GetEngineHandle(), coordId);
        }

        public int CountWordEntries()
        {
            return GrammarEngineApi.sol_CountEntries(_engine);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int FindCoord(string coordName)
        {
            return GrammarEngineApi.sol_FindEnum(_engine, coordName);
        }


        public int FindEntry(string entryName, int partOfSpeech)
        {
            return GrammarEngineApi.sol_FindEntry(_engine, entryName, partOfSpeech, -1);
        }

        public int FindPartOfSpeech(string partOfSpeechName)
        {
            return GrammarEngineApi.sol_FindClass(_engine, partOfSpeechName);
        }

        public int FindPhrase(string phraseText, bool caseSensitive)
        {
            return GrammarEngineApi.sol_FindPhrase(_engine, phraseText, caseSensitive ? 1 : 0);
        }

        public int FindState(int coordId, string stateName)
        {
            return GrammarEngineApi.sol_FindEnumState(_engine, coordId, stateName);
        }

        public int FindTag(string tagName)
        {
            return GrammarEngineApi.sol_FindTagW(GetEngineHandle(), tagName);
        }

        public int FindTagValue(int tagId, string valueName)
        {
            return GrammarEngineApi.sol_FindTagValueW(GetEngineHandle(), tagId, valueName);
        }

        public WordProjections FindWordForm(string wordform)
        {
            return new WordProjections(_engine, GrammarEngineApi.sol_ProjectWord(_engine, wordform, 0));
        }


        public List<string> GenerateWordforms(int entryId, List<int> coordId, List<int> stateId)
        {
            int npairs = coordId.Count;
            int[] pairs = new int[npairs * 2];
            for (int i = 0, j = 0; i < npairs; ++i)
            {
                pairs[j++] = coordId[i];
                pairs[j++] = stateId[i];
            }

            List<string> res = new List<string>();
            IntPtr hStr = GrammarEngineApi.sol_GenerateWordforms(_engine, entryId, npairs, pairs);
            if (hStr != (IntPtr)0)
            {
                int nstr = GrammarEngineApi.sol_CountStrings(hStr);
                for (int k = 0; k < nstr; ++k)
                {
                    res.Add(GrammarEngineApi.sol_GetStringFX(hStr, k));
                }

                GrammarEngineApi.sol_DeleteStrings(hStr);
            }

            return res;
        }

        public string GetClassName(int partOfSpeechId)
        {
            return GrammarEngineApi.sol_GetClassNameFX(GetEngineHandle(), partOfSpeechId);
        }

        public string GetCoordName(int coordId)
        {
            return GrammarEngineApi.sol_GetCoordNameFX(GetEngineHandle(), coordId);
        }

        public string GetCoordStateName(int coordId, int stateId)
        {
            return GrammarEngineApi.sol_GetCoordStateNameFX(GetEngineHandle(), coordId, stateId);
        }

        public int GetCoordType(int partOfSpeechId, int coordId)
        {
            return GrammarEngineApi.sol_GetCoordType(GetEngineHandle(), coordId, partOfSpeechId);
        }

        public IntPtr GetEngineHandle()
        {
            return _engine;
        }


        public int GetEntryAttrState(int entryId, int coordId)
        {
            return GrammarEngineApi.sol_GetEntryCoordState(_engine, entryId, coordId);
        }


        public int GetEntryClass(int idEntry)
        {
            return GrammarEngineApi.sol_GetEntryClass(_engine, idEntry);
        }

        public string GetEntryName(int idEntry)
        {
            return GrammarEngineApi.sol_GetEntryNameFX(_engine, idEntry);
        }


        public List<int> GetLinks(int idEntry, int linkType)
        {
            List<int> res = new List<int>();

            IntPtr hList = GrammarEngineApi.sol_ListLinksTxt(_engine, idEntry, linkType, 0);
            if (hList != IntPtr.Zero)
            {
                int n = GrammarEngineApi.sol_LinksInfoCount(_engine, hList);
                for (int i = 0; i < n; ++i)
                {
                    int idEntry2 = GrammarEngineApi.sol_LinksInfoEKey2(_engine, hList, i);
                    res.Add(idEntry2);
                }

                GrammarEngineApi.sol_DeleteLinksInfo(_engine, hList);
            }

            return res;
        }

        public string GetNounForm(int id, int number, int @case)
        {
            var sb = new StringBuilder();
            GrammarEngineApi.sol_GetNounForm(GetEngineHandle(), id, number, @case, sb);
            return sb.ToString();
        }

        public string GetVerbForm(int id, int number, int gender, int tense, int person)
        {
            var sb = new StringBuilder();
            GrammarEngineApi.sol_GetVerbForm(GetEngineHandle(), id, number, gender, tense, person, sb);
            return sb.ToString();
        }

        public int TranslateToInfinitive(int id)
        {
            return GrammarEngineApi.sol_TranslateToInfinitive(GetEngineHandle(), id);
        }

        public List<int> GetPhrasalLinks(int idPhrase, int linkType)
        {
            List<int> res = new List<int>();

            IntPtr hList = GrammarEngineApi.sol_ListLinksTxt(_engine, idPhrase, linkType, 1);
            if (hList != IntPtr.Zero)
            {
                int n = GrammarEngineApi.sol_LinksInfoCount(_engine, hList);
                for (int i = 0; i < n; ++i)
                {
                    int idPhrase2 = GrammarEngineApi.sol_LinksInfoEKey2(_engine, hList, i);
                    res.Add(idPhrase2);
                }

                GrammarEngineApi.sol_DeleteLinksInfo(_engine, hList);
            }

            return res;
        }

        public string GetPhraseText(int phraseId)
        {
            return GrammarEngineApi.sol_GetPhraseTextFX(_engine, phraseId);
        }

        public TextSegmenter GetTextFileSegmenter(string filePath, string encoding, int languageId)
        {
            IntPtr h = GrammarEngineApi.sol_CreateSentenceBroker(GetEngineHandle(), filePath, encoding, languageId);
            return new TextSegmenter(this, h);
        }


        public ThesaurusLinks ListLinksTxt(int idEntry, int linkCode, int flags)
        {
            IntPtr hList = GrammarEngineApi.sol_ListLinksTxt(GetEngineHandle(), idEntry, linkCode, flags);
            return new ThesaurusLinks(GetEngineHandle(), hList);
        }

        public string NormalizePhrase(AnalysisResults linkages)
        {
            return GrammarEngineApi.NormalizePhraseFX(_engine, linkages.GetHandle());
        }


        public string RestoreCasing(int entryId, string word)
        {
            return GrammarEngineApi.sol_RestoreCasingFX(_engine, word, entryId);
        }

        public string[] Tokenize(string text, int languageId)
        {
            return GrammarEngineApi.sol_TokenizeFX(GetEngineHandle(), text, languageId);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_engine != IntPtr.Zero)
            {
                GrammarEngineApi.sol_DeleteGrammarEngine(_engine);
            }
        }

        private T LinuxHandler<T>(Func<T> isLinux, Func<T> isOther)
        {
            if (IsLinux)
            {
                return isLinux();
            }
            else
            {
                return isOther();
            }
        }

        private byte[] GetUtf8Bytes(string input) => Encoding.UTF8.GetBytes(input);

        private void UnpackResources()
        {
            string curDir;
            string ass = Assembly.GetExecutingAssembly().Location;
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
            string path = !string.IsNullOrEmpty(curDir) ? Path.Combine(curDir, fileName) : fileName;
            if (File.Exists(path))
            {
                return;
            }

            File.WriteAllBytes(path, bytes);
        }
    }
}