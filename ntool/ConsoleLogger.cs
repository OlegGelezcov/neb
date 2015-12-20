using System;
using System.Collections.Generic;

namespace ntool {
    public class ConsoleLogger : ILogger {

        private Stack<ConsoleColor> m_Colors;
        private object m_Sync = new object();

        public ConsoleLogger() {
            m_Colors = new Stack<System.ConsoleColor>();
        }

        public void Log(string message ) {
            Log("{0}", message);
        }

        public void Log(string messageFormat, params object[] args) {
            lock(m_Sync) {
                ConsoleColor oldColor = Console.ForegroundColor;
                if (m_Colors.Count > 0) {
                    Console.ForegroundColor = m_Colors.Peek();
                }
                Console.WriteLine(messageFormat, args);
                Console.ForegroundColor = oldColor;
            }
        }

        public void PopColor() {
            lock(m_Sync) {
                if(m_Colors.Count > 0 ) {
                    m_Colors.Pop();
                }
            }
        }

        public void PushColor(ConsoleColor color) {
            lock(m_Sync) {
                m_Colors.Push(color);
            }
        }
    }
}
