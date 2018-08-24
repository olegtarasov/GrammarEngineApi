using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrammarEngineApi.Api;

namespace GrammarEngineApi.Compiler
{
    /// <summary>
    /// Compiles binary dictionary from sources.
    /// </summary>
    public class DictionaryCompiler
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public DictionaryCompiler()
        {
            GrammarApi.LoadNativeLibrary();
        }

        /// <summary>
        /// Fired when log is updated.
        /// </summary>
        public event EventHandler<string> Error;

        /// <summary>
        /// Compiles binary dictionary from sources to dest path.
        /// </summary>
        /// <param name="sourcePath">Dictionary sources path.</param>
        /// <param name="destPath">Binary destination path.</param>
        public async Task CompileAsync(string sourcePath, string destPath)
        {
            var ass = Assembly.GetExecutingAssembly().Location;
            string curDir = string.IsNullOrEmpty(ass) ? Environment.CurrentDirectory : Path.GetDirectoryName(ass);
            string args = $@"-j=2 -optimize -dir=""{sourcePath}"" -outdir=""{destPath}"" -ldsize=3000000 -save_paradigmas -save_prefix_entry_searcher -save_seeker -save_affixes -save_lemmatizer ""{Path.Combine(sourcePath, "version-pro")}"" ""{Path.Combine(sourcePath, "dictionary")}"" -file=""{Path.Combine(sourcePath, "russian-language-only.sol")}"" ""{Path.Combine(sourcePath, "shared-resources")}"" ""{Path.Combine(sourcePath, "russian-lexicon")}"" ""{Path.Combine(sourcePath, "russian-stat")}"" ""{Path.Combine(sourcePath, "common-syntax")}""  ""{Path.Combine(sourcePath, "russian-syntax")}"" ""{Path.Combine(sourcePath, "russian-thesaurus")}"" ""{Path.Combine(sourcePath, "dictionary-russian")}"" ""{Path.Combine(sourcePath, "common_dictionary_xml")}""";
            string logPath = Path.Combine(destPath, "journal");

            var process = new Process
                          {
                              StartInfo = new ProcessStartInfo("compiler.exe", args)
                                          {
                                              WorkingDirectory = curDir,
                                              CreateNoWindow = true,
                                              RedirectStandardError = true,
                                              StandardErrorEncoding = Encoding.UTF8,
                                              UseShellExecute = false
                                          },
                          };

            process.ErrorDataReceived += (sender, eventArgs) =>
            {
                OnError(eventArgs.Data);
            };

            process.Start();
            process.BeginErrorReadLine();

            await Task.Run(() => process.WaitForExit());
        }


        protected virtual void OnError(string e)
        {
            Error?.Invoke(this, e);
        }
    }
}