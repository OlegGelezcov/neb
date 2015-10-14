using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerActivatorData = Space.Game.Resources.Zones.ActivatorData;
using ClientActivatorData = Nebula.Client.ActivatorData;
using Common;

namespace NebulaTests {
    [TestClass]
    public class ServerClientObjectMappingTests {
        [TestMethod]
        public void TestActivatorDataMapping() {
            ServerActivatorData serverActivator = new ServerActivatorData {
                Action = "TestActiob",
                Id = Guid.NewGuid().ToString(),
                Position = new float[] { 1, 2, 3 },
                Radius = 10,
                Type = 0
            };

            ClientActivatorData clientActivator = new ClientActivatorData();
            clientActivator.ParseInfo(serverActivator.GetInfo());

            Assert.AreEqual(serverActivator.Action, clientActivator.Action);
            Assert.AreEqual(serverActivator.Id, clientActivator.Id);
            Assert.AreEqual(serverActivator.Position[0], clientActivator.Position[0], 0.05f);
            Assert.AreEqual(serverActivator.Position[1], clientActivator.Position[1], 0.05f);
            Assert.AreEqual(serverActivator.Position[2], clientActivator.Position[2], 0.05f);
            Assert.AreEqual(serverActivator.Radius, clientActivator.Radius, 0.05f);
            Assert.AreEqual(serverActivator.Type, clientActivator.Type);
        }
    }
}
