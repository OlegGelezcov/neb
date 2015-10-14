#define LOG_ENABLED

namespace Space.Game
{
    
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Collections;
    using Common;
    using System.Linq;
    using ExitGames.Logging;

    public static class CL
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static void Out(LogFilter filter, string str)
        {
            log.Info(filter.ToString() + ": " + str);
            //if(ConsoleLogging.Get != null )
            //{
            //    ConsoleLogging.Get.Print(filter, str);
            //}
        }

        public static void Out(string str)
        {
            log.Info(LogFilter.DEFAULT + ": " + str);
            //if(ConsoleLogging.Get != null )
            //{
            //    ConsoleLogging.Get.Print(LogFilter.DEFAULT, str);
            //}
        }

        public static void Out(System.Func<bool> func, string str )
        {
            if(func != null )
            {
                if(func())
                {
                    log.Info(LogFilter.DEFAULT + ": " + str);

                    //if (ConsoleLogging.Get != null)
                    //{
                    //    ConsoleLogging.Get.Print(LogFilter.DEFAULT, str);
                    //}
                }
            }
        }

        public static void Out(LogFilter filter, Func<bool> filterCondition, string message)
        {
            if(filterCondition != null)
            {
                if(filterCondition())
                {
                    log.Info(filter.ToString() + ": " + message);
                    //if(ConsoleLogging.Get != null)
                    //{
                    //    ConsoleLogging.Get.Print(filter, message);
                    //}
                }
            }
        }
    }
    

}