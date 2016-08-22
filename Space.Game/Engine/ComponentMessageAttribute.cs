using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Engine {
    [AttributeUsage(AttributeTargets.Method)]
    public class ComponentMessageAttribute : System.Attribute {
        public readonly bool WriteCallLog;

        public ComponentMessageAttribute(bool writeCallLog) {
            WriteCallLog = writeCallLog;
        }

        private string s_Log;

        public string Log {
            get {
                return s_Log;
            }
            set {
                s_Log = value;
            }
        }

    }
}
