using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntool {
    public interface ILogger {
        void PushColor(ConsoleColor color);
        void PopColor();

        void Log(string message, ConsoleColor color);
        void Log(string message);
        void Log(string messageFormat, params object[] args);
    }
}
