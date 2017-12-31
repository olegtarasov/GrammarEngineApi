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
            Parser.Run(args, new App());
        }
    }
}
