using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Timers;
using System.Collections.Concurrent;

namespace TestMessaging {

    public class Item {
        public void Update() { }
    }

    public class ItemCache {

        public ConcurrentDictionary<string, Item> Items;

    }

    public class AllItems {
        public ConcurrentDictionary<byte, ItemCache> ItemCaches;
    }


    class Program {

        public static Random rnd = new Random((int)DateTime.UtcNow.Ticks);

        private static object lockobj = new object();
        public static DateTime dt;

        private static float rNum(float min, float max) {
            return (int)(min + rnd.NextDouble() * (max - min));
        }

        private static string rStr() {
            return string.Format("{0},{1},{2}", rNum(-500, 500), rNum(-500, 500), rNum(-500, 500));
        }

        static void Main(string[] args) {

            for(int i = 0; i < 60; i++) {
                Console.WriteLine(rStr());
            }

            //AllItems allItems = new AllItems();

            //foreach(var typedItems in allItems.ItemCaches) {
            //    foreach(var item in typedItems.Value.Items) {
            //        item.Value.Update();
            //    }
            //}
        }

        private static void T_Elapsed(object sender, ElapsedEventArgs e) {
            lock(lockobj) {
                Console.WriteLine("start at: {0}, current time {1}", System.Threading.Thread.CurrentThread.ManagedThreadId, (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds);
                int a = 1, b = 2, c = 0;
                for(int i = 0; i < 1000000; i++) {
                    for (int j = 0; j < 10; j++) {
                        c += (a + b);
                        c *= 2;
                        c -= (c / 2);
                        string s = c.ToString();
                    }
                }
                
                double sec = (e.SignalTime - dt).TotalSeconds;
                dt = e.SignalTime;
                Console.WriteLine("completed at : {0}, time: {1}", System.Threading.Thread.CurrentThread.ManagedThreadId, sec);
            }
        }
    }

    public class Behaviour {

        public void SendMessage(string message, object arg) {

        }


    }

    public class SomBehaviour : Behaviour {
        private void PrivateMethod() {
            Console.WriteLine("private method");
        }

        void DefaultMethod(object obj) {
            Console.WriteLine("default method");
        }
    }

    public class SomeBehaviour2 : SomBehaviour {

    }
}
