using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nebula.Contracts;
using Common;
using System.Windows.Forms;

namespace Space.Game.Tests {
    [TestClass]
    public class ContractDataCollectionTests {

        private static readonly ContractResource contractResource = new ContractResource();

        [ClassInitialize]
        public static void ClassInit(TestContext context) {
            //MessageBox.Show("ClassInit");
            contractResource.Load("Data/contracts.xml");
        }

        [TestMethod]
        public void TestContractCount() {
            Assert.AreEqual(6, contractResource.contracts.count);
        }

        [TestMethod]
        public void TestCountOfContractsForLevel() {
            Assert.AreEqual(3, contractResource.contracts.GetContractCount(3));
            Assert.AreEqual(6, contractResource.contracts.GetContractCount(10));
            Assert.AreEqual(0, contractResource.contracts.GetContractCount(0));
            //Assert.a
        }

        [TestMethod]
        public void TestGroupCountForRace() {
            var contract = contractResource.contracts.GetContract("ct001");
            KillNPCGroupContractData killNPCGroupContract = contract as KillNPCGroupContractData;
            Assert.AreEqual(4, killNPCGroupContract.GetGroupCount(Race.Humans));
            Assert.AreEqual(4, killNPCGroupContract.GetGroupCount(Race.Criptizoids));
            Assert.AreEqual(3, killNPCGroupContract.GetGroupCount(Race.Borguzands));
        }

        [TestMethod]
        public void TestGroupCountForRaceAndLevel() {
            var contract = contractResource.contracts.GetContract("ct001");
            KillNPCGroupContractData killNPCGroupContract = contract as KillNPCGroupContractData;
            Assert.AreEqual(1, killNPCGroupContract.GetGroupCount(Race.Humans, 1));
            Assert.AreEqual(2, killNPCGroupContract.GetGroupCount(Race.Humans, 2));
            Assert.AreEqual(1, killNPCGroupContract.GetGroupCount(Race.Borguzands, 1));
            Assert.AreEqual(3, killNPCGroupContract.GetGroupCount(Race.Criptizoids, 5));
        }
    }
}
