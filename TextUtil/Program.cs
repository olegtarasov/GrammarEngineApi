﻿using System;
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
            var startDate = DateTime.Now;
            Console.WriteLine($"Started at {startDate:u}");
            Parser.Run(args, new App());
            var endDate = DateTime.Now;
            Console.WriteLine($"Stopped at {endDate:u}. Elapsed {(endDate - startDate):G}");
            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
