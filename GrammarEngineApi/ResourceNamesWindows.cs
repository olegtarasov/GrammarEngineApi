namespace GrammarEngineApi
{
    public static class ResourceNamesWindows
    {
        public const string Sqlite3 = "sqlite3.dll";
        public const string BoostDateTime = "boost_date_time-vc141-mt-x64-1_68.dll";
        public const string BoostRegex = "boost_regex-vc141-mt-x64-1_68.dll";
        public const string BoostSystem = "boost_system-vc141-mt-x64-1_68.dll";
        public const string Compiler = "compiler.exe";
        public const string GrammarEngine = "solarix_grammar_engine.dll";

        public static string Resource(string name) => $"Resources.{name}";
    }
}