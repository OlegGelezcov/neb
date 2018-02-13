using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Quests {
    public class QuestOwnerCollection {

        public Dictionary<string, QuestOwnerData> QuestOwners { get; private set; } = new Dictionary<string, QuestOwnerData>();

        public void Load(string xml) {
            UniXmlDocument document = new UniXmlDocument(xml);
            QuestOwners.Clear();
            foreach(UniXMLElement element in document.Element("quest_owners").Elements("owner")) {
                QuestOwnerData data = new QuestOwnerData();
                data.Load(element);
                QuestOwners[data.Id] = data;
            }
        }

        public QuestOwnerData GetQuestOwnerData(string id) {
            if(QuestOwners.ContainsKey(id)) {
                return QuestOwners[id];
            }
            return null;
        }
    }
}
