using System;


namespace Common {
    public interface ILogContext {
        void Log(LogFilter filter, string message);
        void Log(LogFilter filter, Func<bool> filterCondition, string message);
    }
}
