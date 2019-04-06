using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrammarEngineApi.Api;
using GrammarEngineApi.Properties;
using NativeLibraryManager;

namespace GrammarEngineApi.Compiler
{
    /// <summary>
    /// Compiles binary dictionary from sources.
    /// </summary>
    public class DictionaryCompiler
    {
        private static readonly List<LibraryManager> _libraryManagers;

        static DictionaryCompiler()
        {
            _libraryManagers = new List<LibraryManager>
                               {
                                   new LibraryManager("sqlite3", new LibraryItem("sqlite3.dll", Resources.sqlite3, Platform.Windows, Bitness.x64)),
                                   new LibraryManager("boost_date_time", new LibraryItem("boost_date_time-vc141-mt-x64-1_68.dll", Resources.boost_date_time, Platform.Windows, Bitness.x64)),
                                   new LibraryManager("boost_regex", new LibraryItem("boost_regex-vc141-mt-x64-1_68.dll", Resources.boost_regex, Platform.Windows, Bitness.x64)),
                                   new LibraryManager("boost_system", new LibraryItem("boost_system-vc141-mt-x64-1_68.dll", Resources.boost_system, Platform.Windows, Bitness.x64)),
                                   new LibraryManager("compiler", new LibraryItem("compiler.exe", Resources.compiler, Platform.Windows, Bitness.x64))
                               };
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        public DictionaryCompiler()
        {
            _libraryManagers.ForEach(x => x.LoadNativeLibrary());
        }

        public event EventHandler<string> Log; 

        /// <summary>
        /// Compiles binary dictionary from sources to dest path.
        /// </summary>
        /// <param name="sourcePath">Dictionary sources path.</param>
        /// <param name="destPath">Binary destination path.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task CompileAsync(string sourcePath, string destPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            string args = $@"-j=2 -optimize -dir=""{sourcePath}"" -outdir=""{destPath}"" -ldsize=3000000 -save_paradigmas -save_prefix_entry_searcher -save_seeker -save_affixes -save_lemmatizer ""{Path.Combine(sourcePath, "version-pro")}"" ""{Path.Combine(sourcePath, "dictionary")}"" -file=""{Path.Combine(sourcePath, "russian-language-only.sol")}"" ""{Path.Combine(sourcePath, "shared-resources")}"" ""{Path.Combine(sourcePath, "russian-lexicon")}"" ""{Path.Combine(sourcePath, "russian-stat")}"" ""{Path.Combine(sourcePath, "common-syntax")}""  ""{Path.Combine(sourcePath, "russian-syntax")}"" ""{Path.Combine(sourcePath, "russian-thesaurus")}"" ""{Path.Combine(sourcePath, "dictionary-russian")}"" ""{Path.Combine(sourcePath, "common_dictionary_xml")}""";
            await CompileInternal(sourcePath, destPath, cancellationToken, args);
        }

        /// <summary>
        /// Compiles AN EMPTY binary dictionary from sources to dest path.
        /// </summary>
        /// <param name="sourcePath">Dictionary sources path.</param>
        /// <param name="destPath">Binary destination path.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task CompileEmptyAsync(string sourcePath, string destPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            string args = $@"-j=2 -optimize -dir=""{sourcePath}"" -outdir=""{destPath}"" -ldsize=10000 -save_prefix_entry_searcher -save_seeker -save_affixes ""{Path.Combine(sourcePath, "version-pro")}"" ""{Path.Combine(sourcePath, "dictionary")}"" ""{Path.Combine(sourcePath, "common_dictionary_xml")}"" ""{Path.Combine(sourcePath, "empty_dictionary_xml")}""";
            await CompileInternal(sourcePath, destPath, cancellationToken, args);
        }

        private async Task CompileInternal(string sourcePath, string destPath, CancellationToken cancellationToken, string args)
        {
            var ass = Assembly.GetExecutingAssembly().Location;
            string curDir = string.IsNullOrEmpty(ass) ? Environment.CurrentDirectory : Path.GetDirectoryName(ass);
            string compilerPath = Path.Combine(curDir, "compiler.exe");
            string logPath = Path.Combine(destPath, "journal");

            var process = new Process
                          {
                              StartInfo = new ProcessStartInfo(compilerPath, args)
                                          {
                                              WorkingDirectory = curDir,
                                              CreateNoWindow = true,
                                              RedirectStandardError = true,
                                              StandardErrorEncoding = Encoding.UTF8,
                                              UseShellExecute = false
                                          },
                          };

            await Task.Run(async () =>
            {
                var sb = new StringBuilder();
                process.ErrorDataReceived += (sender, eventArgs) =>
                {
                    if (!string.IsNullOrEmpty(eventArgs.Data))
                    {
                        sb.AppendLine(eventArgs.Data);
                    }
                };

                process.Start();
                process.BeginErrorReadLine();

                long offset = 0;
                while (!process.HasExited)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        process.Kill();
                        throw new TaskCanceledException();
                    }

                    if (File.Exists(logPath))
                    {
                        using (var stream = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var reader = new StreamReader(stream))
                        {
                            stream.Seek(offset, SeekOrigin.Begin);
                            string log = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(log))
                            {
                                OnLog(log);
                            }

                            offset = stream.Position;
                        }
                    }

                    await Task.Delay(100);
                }

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException("Compilation failed! Errors are:" + Environment.NewLine + sb.ToString());
                }
            });
        }

        protected virtual void OnLog(string e)
        {
            Log?.Invoke(this, e);
        }
    }
}