using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Achievments {
    public class AchievmentDataCollection  {
        public ConcurrentDictionary<string, AchievmentCategoryData> categories { get; private set; }

        public AchievmentDataCollection() {
            categories = new ConcurrentDictionary<string, AchievmentCategoryData>();
        }

        public void Load(string path) {
            XDocument document = XDocument.Load(path);

            categories = new ConcurrentDictionary<string, AchievmentCategoryData>();
            var dump = document.Element("achievments").Elements("category").Select(categoryElement => {
                AchievmentCategoryData category = new AchievmentCategoryData(categoryElement);
                categories.TryAdd(category.id, category);
                return category;
            }).ToList();
        }

        public List<AchievmentData> GetAchievmentsForVariable(string variable) {
            List<AchievmentData> list = new List<AchievmentData>();
            foreach(var category in categories ) {
                list.AddRange(category.Value.GetAchievmentsForVariable(variable));
            }
            return list;
        }
    }
}
