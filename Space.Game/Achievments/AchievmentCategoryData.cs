using Common;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Achievments {
    public class AchievmentCategoryData {
        public string id { get; private set; }
        public ConcurrentDictionary<string, AchievmentData> achievments { get; private set; }

        public AchievmentCategoryData(XElement element) {
            id = element.GetString("id");
            achievments = new ConcurrentDictionary<string, AchievmentData>();
            var dump = element.Elements("achievment").Select(aElement => {
                AchievmentData aData = new AchievmentData(aElement);
                achievments.TryAdd(aData.id, aData);
                return aData;
            }).ToList();
        }

        public AchievmentData GetAchievmentData(string id ) {
            AchievmentData aData;
            if(achievments.TryGetValue(id, out aData)) {
                return aData;
            }
            return null;
        }

        public List<AchievmentData> GetAchievmentsForVariable(string variable) {
            List<AchievmentData> list = new List<AchievmentData>();
            foreach(var kvp in achievments) {
                if(kvp.Value.variable == variable ) {
                    list.Add(kvp.Value);
                }
            }
            return list;
        }
    }
}
