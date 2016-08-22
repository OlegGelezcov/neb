using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula {
    public interface ILibLogger {
        void Log(string msg);
    }

    public static class LibLogger {
        private static ILibLogger s_Logger;

        public static void SetLibLogger(ILibLogger logger) {
            s_Logger = logger;
        }

        public static void Log(string message) {
            if(s_Logger != null ) {
                s_Logger.Log(message);
            }
        }
    }
}
