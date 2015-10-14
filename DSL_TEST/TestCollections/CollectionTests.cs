using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSL_TEST.TestCollections {
    public static class CollectionTests {
        private const int PAGE_SIZE = 3;

        public static void TestEmpty() {

            Dictionary<int, int> dict = new Dictionary<int, int>();
            int mPage = 10;
            var result = dict.Skip(mPage * PAGE_SIZE).Take(PAGE_SIZE).ToDictionary(pk => pk.Key, pk => pk.Value);
            Console.WriteLine("empty count with page = 10 = {0}", result.Count);

            for(int i = 0; i < 10; i++) {
                dict.Add(i, i + 1);
            }

            result = dict.Skip(mPage * PAGE_SIZE).Take(PAGE_SIZE).ToDictionary(pk => pk.Key, pk => pk.Value);
            Console.WriteLine("10 count with page = 10 = {0}", result.Count);

            mPage = 0;
            result = dict.Skip(mPage * PAGE_SIZE).Take(PAGE_SIZE).ToDictionary(pk => pk.Key, pk => pk.Value);
            Console.WriteLine("10 count with page = 0 = {0}", result.Count);

            mPage = 3;
            result = dict.Skip(mPage * PAGE_SIZE).Take(PAGE_SIZE).ToDictionary(pk => pk.Key, pk => pk.Value);
            Console.WriteLine("10 count with page = 3 = {0}", result.Count);
        }
    }
}
