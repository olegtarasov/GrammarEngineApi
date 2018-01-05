using System.Linq;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace TextUtil
{
    public static class LogHelper
    {
        private static readonly object _locker = new object();
        
        public static void ConfigureLog4Net(Assembly assembly, bool file = true, bool console = false, string fileName = "Logs\\Log.txt")
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository(assembly);
            if (hierarchy.Configured)
            {
                return;
            }

            lock (_locker)
            {
                hierarchy = (Hierarchy)LogManager.GetRepository(assembly);
                if (hierarchy.Configured)
                {
                    return;
                }
                
                var patternLayout = new PatternLayout {ConversionPattern = "%d %-5p %logger{1} - %m%n%exception"};
                patternLayout.ActivateOptions();

                if (file)
                {
                    var roller = new RollingFileAppender
                                 {
                                     AppendToFile = true,
                                     File = fileName,
                                     Layout = patternLayout,
                                     MaxSizeRollBackups = 5,
                                     MaximumFileSize = "5MB",
                                     RollingStyle = RollingFileAppender.RollingMode.Size,
                                     Encoding = Encoding.UTF8,
                                     LockingModel = new FileAppender.MinimalLock()
                                 };
                    roller.ActivateOptions();
                    hierarchy.Root.AddAppender(roller);
                }

                if (console)
                {
                    var consoleAppender = new ConsoleAppender
                                          {
                                              Layout = patternLayout
                                          };
                    consoleAppender.ActivateOptions();
                    hierarchy.Root.AddAppender(consoleAppender);
                }

                hierarchy.Root.Level = Level.Debug;
                hierarchy.Configured = true;
            }
        }

        public static string GetLog4NetFilePath(Assembly assembly)
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository(assembly);
            if (!hierarchy.Configured)
            {
                return null;
            }

            return hierarchy.Root.Appenders.OfType<RollingFileAppender>().FirstOrDefault()?.File;
        }
    }
}