using Common;
using Space.Game;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class NPCGroupDataCollection {
        //private ConcurrentDictionary<string, NPCGroupData> m_Groups;
        private List<NPCGroupData> m_Groups;

        public NPCGroupDataCollection(XElement parent) {
            m_Groups = new List<NPCGroupData>();

            var dump = parent.Elements("group").Select(ge => {
                NPCGroupData group = new NPCGroupData(ge);
                m_Groups.Add(group);
                return group;
            }).ToList();
        }

        public NPCGroupDataCollection() {
            m_Groups = new List<NPCGroupData>();
        }

        private List<NPCGroupData> AvailableGroups(Race race, int level) {
            List<NPCGroupData> filteredGroups = new List<NPCGroupData>();
            foreach(var group in m_Groups ) {
                var g = group;
                if(g.IsValidRace(race) && g.IsValidLevel(level)) {
                    filteredGroups.Add(g);
                }
            }
            return filteredGroups;
        }

        public bool HasGroup(Race race, int level ) {
            return AvailableGroups(race, level).Count > 0;
        }

        public NPCGroupData GetRandomGroup(Race race, int level ) {
            var filtered = AvailableGroups(race, level);
            if(filtered.Count > 0 ) {
                return filtered.AnyElement();
            } else {
                return null;
            }
        }

        public List<NPCGroupData> GetGroups(Race race) {
            return AvailableGroups(race, int.MaxValue);
        }


        public int GetGroupCount(Race race) {
            return GetGroups(race).Count;
        }

        public int GetGroupCount(Race race, int level ) {
            return AvailableGroups(race, level).Count;
        }
    }
}
