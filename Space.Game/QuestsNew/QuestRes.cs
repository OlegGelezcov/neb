using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Quests {
    public class QuestRes {

        public List<QuestData> Quests { get; } = new List<QuestData>();

        public void Load(string fullPath ) {
            Quests.Clear();
            XDocument document = XDocument.Load(fullPath);
            document.Element("quests").Elements("quest").ToList().ForEach(q => {
                QuestData questData = new QuestData();
                questData.Load(q);
                Quests.Add(questData);
            });
        }

        public QuestData GetQuest(string id) => Quests.FirstOrDefault(q => q.Id == id);

    }
}
