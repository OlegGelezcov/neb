using ExitGames.Concurrency.Fibers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.TestLoopThrowFibers {
    public class LoopFibersTest {
        private static IDisposable updateLoop;
        private static PoolFiber loopFiber;
        private static DateTime refData = DateTime.UtcNow;

        public static void Run() {
            loopFiber = new PoolFiber();
            loopFiber.Start();

            updateLoop = loopFiber.ScheduleOnInterval(Update, 0, 1000);
        }

        public static void Update() {
            float start = time();
            Console.WriteLine("Start at time: {0}", time());
            for(int i = 0; i < 1000000; i++) {
                for (int j = 0; j < 100; j++) {
                    string s = "a";
                    string s2 = "b";
                    string result = s + s2;
                    result = result + result;
                    int a = 5;
                    result += a.ToString();
                }
            }
            float end = time();
            Console.WriteLine("INTERVAL: {0} thread {1}", end - start, System.Threading.Thread.CurrentThread.ManagedThreadId);
        }


        private static float time() {
            return (float)(DateTime.UtcNow - refData).TotalSeconds;
        }
    }
}
