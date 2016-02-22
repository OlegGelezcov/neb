using Common;
using Space.Game;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class SpecialNPCDataCollection {
        private List<SpecialNPCData> m_NPCs;

        public SpecialNPCDataCollection(XElement parent) {
            m_NPCs = new List<SpecialNPCData>();

            var dump = parent.Elements("npc").Select(npce => {
                SpecialNPCData npc = new SpecialNPCData(npce);
                m_NPCs.Add(npc);
                return npc;
            }).ToList();
        }

        public SpecialNPCDataCollection() {
            m_NPCs = new List<SpecialNPCData>();
        }

        private List<SpecialNPCData> AvailableNPCs(Race race, int level ) {
            List<SpecialNPCData> filteredNPCs = new List<SpecialNPCData>();
            foreach(var npc in m_NPCs) {
                if(npc.IsValidRace(race) && npc.IsValidLevel(level)) {
                    filteredNPCs.Add(npc);
                }
            }
            return filteredNPCs;
        }

        public bool HasNPC(Race race, int level ) {
            return AvailableNPCs(race, level).Count > 0; 
        }

        public SpecialNPCData GetRandomNPC(Race race, int level ) {
            var filtered = AvailableNPCs(race, level);
            if(filtered.Count > 0 ) {
                return filtered.AnyElement();
            }
            return null;
        }

        public List<SpecialNPCData> GetNPCs(Race race) {
            return AvailableNPCs(race, int.MaxValue);
        }

        public int GetNPCCount(Race race) {
            return GetNPCs(race).Count;
        }

        public int GetNPCCount(Race race, int level ) {
            return AvailableNPCs(race, level).Count;
        }
    }
}
