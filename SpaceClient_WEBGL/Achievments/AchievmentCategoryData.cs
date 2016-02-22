using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Achievments {
    public class AchievmentCategoryData {
        public string id { get; private set; }
        public string name { get; private set; }

        public Dictionary<string, AchievmentData> achievments { get; private set; }

        public AchievmentCategoryData(UniXMLElement element) {
            id = element.GetString("id");
            name = element.GetString("name");
            achievments = new Dictionary<string, AchievmentData>();
            var dump = element.element.Elements("achievment").Select(aElement => {
                AchievmentData data = new AchievmentData(new UniXMLElement(aElement));
                achievments.Add(data.id, data);
                return data;
            }).ToList();
        }

        public bool HasAchievment(string id) {
            return achievments.ContainsKey(id);
        }

        public AchievmentData GetAchievment(string id) {
            if(HasAchievment(id)) {
                return achievments[id];
            }
            return null;
        }

        public int totalTierCount {
            get {
                int count = 0;
                foreach(var kvp in achievments) {
                    count += kvp.Value.tierCount;
                }
                return count;
            }
        }

        public int CountOfUnlockedTiers(PlayerAchievmentObject playerAch) {
            int cnt = 0;
            foreach(var kvp in achievments) {
                cnt += kvp.Value.CountOfUnlockedTiers(playerAch);
            }
            return cnt;
        }
    }
}
