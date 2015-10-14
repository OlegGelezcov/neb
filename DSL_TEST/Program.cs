using Common;
using DSL.Lang;
using DSL_TEST.AddingAndRemovingComponents;
using DSL_TEST.InfoStats;
using GameMath;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DSL_TEST {
    class Program {

        private static readonly Random rnd = new Random();

        static void Main(string[] args) {
            /*
            Collector collector = new Collector();
            collector.CollectTests(20);*/
            //TestCollections.CollectionTests.TestEmpty();
            //NetUtilsTest.TestSendToSlack();
            //Console.ReadKey();
            TestAddingAndRemovingComponents();
        }

        private static void TestAddingAndRemovingComponents() {
            AddingAndRemovingComponentsTest testObject = new AddingAndRemovingComponentsTest();
            testObject.Test();
        }

        static void TestXml() {
            XDocument document = XDocument.Load("skills.xml");
            var lst = document.Element("skills").Elements("skill").Select(e => {
                Console.WriteLine("parse: " + e.Attribute("id").Value);
                int Id = int.Parse(e.Attribute("id").Value, NumberStyles.HexNumber);
                float Cooldown = e.GetFloat("cooldown");
                float Durability = e.GetFloat("durability");
                float RequiredEnergy = e.GetFloat("energy");
                return Id;
            }).ToList();

        }

        static void Tokens() {
            string str = @"while $a < 10
                            $a->pow 2 3
                            endwhile
                            if $b > $a 
                                $c = $a + $b
                                if $zzz == 13.4
                                    $g->print 
                                endif
                            else
                                $d = $c - $d
                            endif";
            LangLexer lexer = new LangLexer(str);
            lexer.GetStream().Print();
        }

        public static void TestNormalDistrib() {
            for(int i = 0; i < 100; i++) {
                float u = (float)rnd.NextDouble();
                float v = (float)rnd.NextDouble();
                float x = (float)(Math.Sqrt(-2 * Math.Log(u)) * Math.Cos(2 * Math.PI * v));
                float y = (float)(Math.Sqrt(-2 * Math.Log(u)) * Math.Sin(2 * Math.PI * v));
                Console.WriteLine("x = {0}; y = {1}", x, y);
            }
        }

        public static void TestRandomIndices() {
            float last = 1f - 0.01f - 0.01f - 0.05f - 0.1f;

            float[] weights = new float[] { 0.01f, 0.02f, 0.05f, 0.10f, last };
            int[] indices = new int[] { 0, 0, 0, 0, 0 };

            for(int i = 0; i < 1000; i++) {
                indices[Rand.RandomIndex(weights)]++;
            }

            for(int i = 0; i < indices.Length; i++) {
                Console.WriteLine("ind = {0}  count = {1}", i, indices[i]);
            }
        }
    }
}
