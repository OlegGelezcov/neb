using ExitGames.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula {
    public class LibLoggerObject : ILibLogger {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        public void Log(string msg) {
            s_Log.Info(msg);
        }

        public void Setup() {
            LibLogger.SetLibLogger(this);
        }
    }
}
