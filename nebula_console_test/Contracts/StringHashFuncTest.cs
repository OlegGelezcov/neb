using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace nebula_console_test.Contracts {
    public class StringHashFuncTest {
        private Dictionary<string, string> m_Strings = new Dictionary<string, string>();

        public void Load(string directory) {
            m_Strings.Clear();

            string[] files = Directory.GetFiles(directory, "*.xml");

            foreach(var file in files ) {
                Console.WriteLine("loading:" + file);
                XDocument document = XDocument.Load(file);
                document.Element("strings").Elements("string").Select(e => {
                    string key = e.Attribute("key").Value;
                    string en = e.Attribute("en").Value;
                    if (m_Strings.ContainsKey(key) == false) {
                        m_Strings.Add(key, en);
                    }
                    return key;
                }).ToList();
            }

            Console.WriteLine("loaded: " + m_Strings.Count);
        }

        public void FindMaxCharAtKey() {
            int max = 0;
            char c = '0';

            foreach(string key in m_Strings.Keys ) {
                foreach(char ch in key) {
                    int ich = (int)ch;
                    if(ich > max ) {
                        max = ich;
                        c = ch;
                    }
                }
            }

            Console.WriteLine("Max character: {0} with code: {1}", c, max);
        }

        public void TestSimpleSum() {
            Dictionary<uint, string> hashedKeys = new Dictionary<uint, string>();

            int duplicateCount = 0;

            foreach(string key in m_Strings.Keys) {
                uint hKey = JenkinsOneAtATimeHash(key);
                if(hashedKeys.ContainsKey(hKey)) {
                    Console.WriteLine("Duplicate {0}: {1}", key, hKey);
                    duplicateCount++;
                } else {
                    hashedKeys.Add(hKey, key);
                }
            }

            Console.WriteLine("total duplicate count: {0}", duplicateCount);
        }

        public uint JenkinsOneAtATimeHash(string str) {
            uint hash = 0;
            for(int i = 0; i < str.Length; ++i ) {
                hash += (uint)str[i];
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            return hash;
        }

        private ulong SimpleSumHash(string str) {
            /*
            int sum = 0;

            for(int i = 0; i < str.Length; i++ ) {
                if(i == 0 ) {
                    sum += (int)str[i] * 128 * 128 * 128;
                } else if( i == 1 ) {
                    sum += (int)str[i] * 128 * 128;
                } else if(i == 2 ) {
                    sum += (int)str[i] * 128;
                } else {
                    sum += (int)str[i];
                }
            }
            return sum;*/

            /*
            int hash = 7;
            foreach(char c in str) {
                hash = Math.Abs((hash + 37 * (int)c) % 116107);
            }*/

            ulong hash = 116107;

            ushort c = 0;
            foreach(char ch in str) {
                c = (ushort)ch;
                hash = ((hash << 5) + hash) + c;
            }
            return hash;
        }
    }
}
