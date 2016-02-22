using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Achievments {
    public class AchievmentCategoryDataCollection {
        public Dictionary<string, AchievmentCategoryData> categories { get; private set; }

        public AchievmentCategoryDataCollection() {
            categories = new Dictionary<string, AchievmentCategoryData>();
        }

        public void Load(string xmlText) {
            categories.Clear();
            UniXmlDocument documet = new UniXmlDocument(xmlText);
            var dump = documet.document.Element("achievments").Elements("category").Select(categoryElement => {
                AchievmentCategoryData category = new AchievmentCategoryData(new UniXMLElement(categoryElement));
                categories.Add(category.id, category);
                return category;
            }).ToList();
        }

        public AchievmentData GetAchievment(string id) {
            foreach(var kvp in categories) {
                if(kvp.Value.HasAchievment(id)) {
                    return kvp.Value.GetAchievment(id);
                }
            }
            return null;
        }

        public int totalTierCount {
            get {
                int count = 0;
                foreach(var kvp in categories) {
                    count += kvp.Value.totalTierCount;
                }
                return count;
            }
        }

        public int CountOfUnlockedTiers(PlayerAchievmentObject playerAch) {
            int cnt = 0;
            foreach(var kvp in categories ) {
                cnt += kvp.Value.CountOfUnlockedTiers(playerAch);
            }
            return cnt;
        }
    }
}
