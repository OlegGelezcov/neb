﻿using Nebula.Contracts;
using Nebula.Game.Contracts.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebula_console_test.Contracts {
    public class ContractTests {

        public void TestContractGeneration() {
            ContractResource resource = new ContractResource();
            resource.Load("Data/contracts.xml");

            var generator = ContractGenerator.Create(Common.ContractCategory.killNPCGroup);

            Console.WriteLine("For criptizids of lvl 6:");
            for(int i = 0; i < 10; i++ ) {
                var contract = generator.Generate(Common.Race.Criptizoids, 6, "E1", null, resource);
                Console.WriteLine(contract.ToString());
                Console.WriteLine("---");
            }
            Console.WriteLine("=========");

            Console.WriteLine("For humans of lvl 2:");
            for(int i = 0; i < 10; i++ ) {
                var contract = generator.Generate(Common.Race.Humans, 2, "H15", null, resource);
                Console.WriteLine(contract.ToString());
                Console.WriteLine("---");
            }
            Console.WriteLine("=========");
        }
    }
}
