using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntool.Commands {
    public abstract class BaseCommand {

        private string m_Source;
        private Application m_Application;

        public BaseCommand(string source, Application app) {
            m_Source = source.ToLower().Trim();
            m_Application = app;
        }

        public abstract void Execute();

        private string[] Tokens() {
            return m_Source.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private T ParseArgument<T>(int argumentIndex) {
            var tokens = Tokens();

            try {
                if (tokens.Length > argumentIndex) {
                    if (typeof(T) == typeof(string)) {
                        return (T)(object)tokens[argumentIndex];
                    } else if (typeof(T) == typeof(byte)) {
                        return (T)(object)byte.Parse(tokens[argumentIndex]);
                    } else if (typeof(T) == typeof(int)) {
                        return (T)(object)int.Parse(tokens[argumentIndex]);
                    } else if (typeof(T) == typeof(float)) {
                        return (T)(object)float.Parse(tokens[argumentIndex]);
                    }
                }
                app.logger.PushColor(ConsoleColor.Red);
                app.logger.Log(string.Format("{0} unsupported type for command line", typeof(T).Name));
                app.logger.PopColor();

                throw new ArgumentException(string.Format("{0} unsupported type for command line", typeof(T).Name));
            } catch (Exception exception ) {
                app.logger.PushColor(ConsoleColor.Red);
                app.logger.Log(exception.Message);
                app.logger.PopColor();
                if(typeof(T) == typeof(string)) {
                    return (T)(object)string.Empty;
                } else if(typeof(T) == typeof(byte)) {
                    return (T)(object)(byte)0;
                } else if( typeof(T) == typeof(int)) {
                    return (T)(object)(int)0;
                } else if(typeof(T) == typeof(float)) {
                    return (T)(object)(float)0f;
                }
                return default(T);
            }
        }

        public void GetCommandArguments<T>(out T arg) {
            //app.logger.Log("call 1");
            arg = ParseArgument<T>(1);
        }

        public void GetCommandArguments<T1, T2>(out T1 arg1, out T2 arg2) {
            //app.logger.Log("call 2");
            GetCommandArguments<T1>(out arg1);
            arg2 = ParseArgument<T2>(2);
        }

        public void GetCommandArguments<T1, T2, T3>(out T1 arg1, out T2 arg2, out T3 arg3) {
            //app.logger.Log("call 3");
            GetCommandArguments<T1, T2>(out arg1, out arg2);
            arg3 = ParseArgument<T3>(3);
        }

        public void GetCommandArguments<T1, T2, T3, T4>(out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4) {
            //app.logger.Log("call 4");
            GetCommandArguments<T1, T2, T3>(out arg1, out arg2, out arg3);
            arg4 = ParseArgument<T4>(4);
        }

        public void GetCommandArguments<T1, T2, T3, T4, T5>(out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5) {
           // app.logger.Log("call 5");
            GetCommandArguments<T1, T2, T3, T4>(out arg1, out arg2, out arg3, out arg4);
            arg5 = ParseArgument<T5>(5);
        }



        private static string CommandName(string source) {
            string[] tokens = source.ToLower().Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if(tokens.Length > 0 ) {
                return tokens[0];
            }
            return string.Empty;
        }

        public static BaseCommand CreateCommand(string source, Application context) {
            string commandName = CommandName(source);
            switch(commandName) {
                case "login":
                    return new LoginCommand(source, context);
                case "register":
                    return new RegisterCommand(source, context);
                case "getwebchar":
                    return new GetWebCharacterObjectCommand(source, context);

            }
            return null;
        }

        public Application app {
            get {
                return m_Application;
            }
        }
    }
}
