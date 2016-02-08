using nebula_console_test.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebula_console_test {
    class Program {
        static void Main(string[] args) {
            ContractTests tests = new ContractTests();
            //tests.TestContractGeneration();
            //tests.TestKillNPCGroupContractServerClientCompativility();
            tests.TestExploreLocationParsing();
        }
    }
}
