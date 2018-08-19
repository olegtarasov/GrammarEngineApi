using System;
using CLAP;
using GrammarEngineApi.Compiler;

namespace TextUtil
{
    public partial class App
    {
        [Verb]
        public void CompileDictionary(string source, string target)
        {
            var compiler = new DictionaryCompiler();
            compiler.Error += (sender, log) =>
            {
                Console.WriteLine(log);
            };

            compiler.CompileAsync(source, target).Wait();
        }
    }
}