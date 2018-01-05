using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLAP;

namespace TextUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            LogHelper.ConfigureLog4Net(typeof(Program).Assembly, false, true);
            Parser.Run(args, new App());
            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
