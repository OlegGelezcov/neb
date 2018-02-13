using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Quests {
    public class QuestDataCollection {

        public Dictionary<string, Nebula.Client.Quests.QuestData> Quests { get; private set; } = new Dictionary<string, Client.Quests.QuestData>();

        public void Load(string xmlTex) {

            UniXmlDocument document = new UniXmlDocument(xmlTex);

            Quests.Clear();
            foreach(var questElement in document.Element("quests").Elements("quest")) {
                Nebula.Client.Quests.QuestData questData = new Client.Quests.QuestData();
                questData.Load(questElement);
                Quests[questData.Id] = questData;
            }
        }

        public Nebula.Client.Quests.QuestData GetQuestData(string id) {
            if(Quests.ContainsKey(id)) {
                return Quests[id];
            }
            return null;
        }
    }
}
