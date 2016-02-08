using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class KillNPCContractData : ContractData {
        private SpecialNPCDataCollection m_NPCs;

        public KillNPCContractData(XElement element) 
            : base(element) {
            var npcsElement = element.Element("npcs");
            m_NPCs = new SpecialNPCDataCollection(npcsElement);
        }

        public int GetNPCCount(Race race) {
            return m_NPCs.GetNPCCount(race);
        }
        public int GetNPCCount(Race race, int level ) {
            return m_NPCs.GetNPCCount(race, level);
        }
        public bool HasNPCs(Race race, int level ) {
            return m_NPCs.GetNPCCount(race, level) > 0;
        }
        public SpecialNPCData GetRandomNPC(Race race, int level) {
            return m_NPCs.GetRandomNPC(race, level);
        }
    }
}
