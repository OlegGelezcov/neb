using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntool {
    public class CommandManager {

        private bool m_Running = false;
        private Application m_Application;

        public CommandManager(Application app) {
            m_Application = app;
        }

        public Application app {
            get {
                return m_Application;
            }
        }

        public void Run() {
            m_Running = true;
            var buffer = new StringBuilder();

            while(m_Running) {
                if(Console.KeyAvailable) {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if(key.Key != ConsoleKey.Enter) {
                        if (key.Key != ConsoleKey.Backspace) {
                            buffer.Append(key.KeyChar);
                        }
                    } else {
                        app.logger.PushColor(ConsoleColor.DarkYellow);
                        app.logger.Log("command: " + buffer.ToString().Trim());
                        app.logger.PopColor();
                        app.Command(buffer.ToString().Trim().ToLower());
                        buffer.Clear();
                    }
                }
            }
        }



        public void Stop() {
            m_Running = false;
        }
    }
}
