using System.Collections.Generic;
using System.Linq;

namespace Nebula.Client.Manual {
    public class ManQuestResource {
        public List<ManQuestData> quests { get; private set; }

        public ManQuestResource() {
            quests = new List<ManQuestData>();
        }

        public void Load(string xmlText ) {
            UniXmlDocument document = new UniXmlDocument(xmlText);
            quests = document.document.Element("quests").Elements("quest").Select(qe => {
                return new ManQuestData(new UniXMLElement(qe));
            }).ToList();
        }

        public ManQuestData GetQuest(string id ) {
            foreach(var quest in quests ) {
                if(quest.id == id ) {
                    return quest;
                }
            }
            return null;
        }

        public ManQuestData GetQuest(int index, ManQuestCategory category) {
            foreach(var quest in quests ) {
                if(quest.index == index && quest.type == category ) {
                    return quest;
                }
            }
            return null;
        }
    }
}
