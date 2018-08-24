using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CLAP;
using GrammarEngineApi.Compiler;

namespace TextUtil
{
    public partial class App
    {
        [Verb]
        public void CompileDictionary(string source, string target)
        {
            Console.WriteLine("== Hit any key to cancel ==");

            var cts = new CancellationTokenSource();
            var compiler = new DictionaryCompiler();
            bool done = false;

            compiler.Log += (sender, s) => Console.Write(s);
            var t = compiler.CompileAsync(source, target, cts.Token)
                            .ContinueWith(task =>
                            {
                                done = true;
                                if (task.Exception != null)
                                {
                                    Console.WriteLine(task.Exception.Flatten().ToString());
                                }
                                else
                                {
                                    _log.Info("Compiled successfully. Hit any key now.");
                                }
                            });

            Console.ReadKey();

            if (!done)
            {
                Console.WriteLine("Cancelling");
                cts.Cancel();
            }

            try
            {
                t.Wait();
            }
            catch
            {
            }
        }
    }
}