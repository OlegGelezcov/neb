using Common;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Achievments {
    public class AchievmentData {
        public string id { get; private set; }
        public string variable { get; private set; }
        public ConcurrentDictionary<int, AchievmentTierData> tiers { get; private set; }

        public AchievmentData(XElement element) {
            id = element.GetString("id");
            variable = element.GetString("var");
            tiers = new ConcurrentDictionary<int, AchievmentTierData>();
            var dump = element.Elements("tier").Select(tierElement => {
                AchievmentTierData tierData = new AchievmentTierData(tierElement);
                tiers.TryAdd(tierData.id, tierData);
                return tierData;
            }).ToList();
        }

        public AchievmentTierData GetTierData(int id) {
            AchievmentTierData tier;
            if(tiers.TryGetValue(id, out tier)) {
                return tier;
            }
            return null;
        }

        public bool TierUnlockedFor(int tierId, int counter ) {
            var tierData = GetTierData(tierId);
            if(tierData != null ) {
                return tierData.UnlockedFor(counter);
            }
            return false;
        }

        public List<AchievmentTierData> GetUnlockedByCountTiers(int oldCount, int newCount ) {
            List<AchievmentTierData> list = new List<AchievmentTierData>();
            foreach(var kvp in tiers ) {
                if(kvp.Value.IsUnlockedByValues(oldCount, newCount)) {
                    list.Add(kvp.Value);
                }
            }
            return list;
        }
    }
}
