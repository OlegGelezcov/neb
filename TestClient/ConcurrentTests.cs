using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient {
    public class ConcurrentTests {

        public static void TestConcurrentBag()
        {
            ConcurrentBag<string> sBag = new ConcurrentBag<string>();
            sBag.Add("one");
            sBag.Add("two");
            sBag.Add("three");

            foreach (var s in sBag) { Console.WriteLine(s); }
        }
    }
}
