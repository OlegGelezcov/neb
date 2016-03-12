using nebula_console_test.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebula_console_test {
    class Program {
        static void Main(string[] args) {
            //ContractTests tests = new ContractTests();
            //tests.TestContractGeneration();
            //tests.TestKillNPCGroupContractServerClientCompativility();
            //tests.TestExploreLocationParsing();
            //new RemapWeightsTest().CollectStatistics(new float[] { 0, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f });
            StringHashFuncTest hashTest = new StringHashFuncTest();
            hashTest.Load("DataClient/Strings");
            hashTest.FindMaxCharAtKey();
            hashTest.TestSimpleSum();
        }
    }
}
