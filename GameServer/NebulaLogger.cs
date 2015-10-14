using ExitGames.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula {
    class NebulaLogger : INebulaLogger {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public void Log(string message) {
            log.Info(log);
        }
    }
}
