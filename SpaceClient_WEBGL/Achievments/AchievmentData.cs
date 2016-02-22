using System.Collections.Generic;
using System.Linq;

namespace Nebula.Client.Achievments {
    public class AchievmentData {
        public string id { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }
        public string variable { get; private set; }

        public Dictionary<int, AchievmentTierData> tiers { get; private set; }


        public AchievmentData(UniXMLElement element) {
            tiers = new Dictionary<int, Achievments.AchievmentTierData>();
            id = element.GetString("id");
            name = element.GetString("name");
            description = element.GetString("description");
            variable = element.GetString("var");
            var dump = element.element.Elements("tier").Select(tierElement => {
                AchievmentTierData tierData = new Achievments.AchievmentTierData(new UniXMLElement(tierElement));
                tiers.Add(tierData.id, tierData);
                return tierData;
            }).ToList();
        }

        public bool AllOpened(PlayerAchievmentObject currentVariables ) {
            var count = currentVariables.GetValue(variable);
            bool ok = true;
            foreach(var kvp in tiers ) {
                if(!kvp.Value.Unlocked(count)) {
                    ok = false;
                    break;
                }
            }
            return ok;
        }

        public AchievmentTierData GetTierData(int id) {
            if(tiers.ContainsKey(id)) {
                return tiers[id];
            }
            return null;
        }

        public int tierCount {
            get {
                return tiers.Count;
            }
        }

        public int CountOfUnlockedTiers(PlayerAchievmentObject playerAch) {
            int playerCount = playerAch.GetValue(variable);
            int cnt = 0;
            foreach(var t in tiers ) {
                if(t.Value.Unlocked(playerCount)) {
                    cnt++;
                }
            }
            return cnt;
        }
    }
    
}
