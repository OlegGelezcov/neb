
namespace Space
{
    using Common;
    using Space.Game;


    public class ConsoleLogContext : ILogContext
    {

        private static ILogContext instance;

        public static ILogContext Instance
        {
            get
            {
                if (instance == null)
                    instance = new ConsoleLogContext();
                return instance;
            }
        }


        public void Log(LogFilter filter, string message)
        {
            CL.Out(filter, message);
        }

        public void Log(LogFilter filter, System.Func<bool> filterCondition, string message)
        {
            CL.Out(filter, filterCondition, message);
        }
    }
}
