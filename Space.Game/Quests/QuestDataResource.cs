using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nebula.Quests {

    public class QuestDataResource {
        private readonly ConcurrentDictionary<Race, QuestDataCollection> m_Quests = new ConcurrentDictionary<Race, QuestDataCollection>();

        public void Load(string basePath) {
            m_Quests.Clear();

            QuestDataCollection humanQuests = new QuestDataCollection();
            humanQuests.Load(Path.Combine(basePath, "Data/quests_h.xml"));
            m_Quests.TryAdd(Race.Humans, humanQuests);

            QuestDataCollection cripQuests = new QuestDataCollection();
            cripQuests.Load(Path.Combine(basePath, "Data/quests_c.xml"));
            m_Quests.TryAdd(Race.Criptizoids, cripQuests);

            QuestDataCollection borgQuests = new QuestDataCollection();
            borgQuests.Load(Path.Combine(basePath, "Data/quests_b.xml"));
            m_Quests.TryAdd(Race.Borguzands, borgQuests);
        }

        public QuestDataCollection GetQuests(Race race) {
            QuestDataCollection result = null;
            if(m_Quests.TryGetValue(race, out result)) {
                return result;
            }
            return null;
        }

        public QuestData GetQuest(Race race, string id) {
            var quests = GetQuests(race);
            if(quests != null ) {
                return quests.GetQuest(id);
            }
            return null;
        }
    }
}
