using Common;
using Nebula.Client.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Quests {
    public class QuestData {
        public string Id { get; private set; }
        public QuestType Type { get; private set; }
        public Dictionary<Race, BaseData> RaceBaseData { get; private set; }

        public List<QuestRewardData> Rewards { get; private set; }

        public Dictionary<Race, QuestConditionCollection> CompleteConditions { get; } = new Dictionary<Race, QuestConditionCollection>();

        public void Load(UniXMLElement element) {
            Id = element.GetString("id");
            Type = element.GetEnum<QuestType>("type");
            RaceBaseData = new Dictionary<Race, BaseData>();
            LoadBaseData(element, Race.Humans, "humans");
            LoadBaseData(element, Race.Borguzands, "borgs");
            LoadBaseData(element, Race.Criptizoids, "krips");

            Rewards = new List<QuestRewardData>();
            foreach(var rewardElement in element.Element("rewards").Elements("reward")) {
                QuestRewardData reward = new QuestRewardData();
                reward.Load(rewardElement);
                Rewards.Add(reward);
            }

            var humansCompleteConditions = element.Element("complete_conditions").Element("humans");
            var borgsCompleteConditions = element.Element("complete_conditions").Element("borgs");
            var kripsCompleteConditions = element.Element("complete_conditions").Element("krips");

            QuestConditionCollection humanComplete = new QuestConditionCollection();
            humanComplete.Load(humansCompleteConditions);

            QuestConditionCollection borgsComple = new QuestConditionCollection();
            borgsComple.Load(borgsCompleteConditions);

            QuestConditionCollection kripsComplete = new QuestConditionCollection();
            kripsComplete.Load(kripsCompleteConditions);

            CompleteConditions[Race.Humans] = humanComplete;
            CompleteConditions[Race.Borguzands] = borgsComple;
            CompleteConditions[Race.Criptizoids] = kripsComplete;
        }

        private void LoadBaseData(UniXMLElement parent, Race race, string elementName ) {
            var raceElement = parent.Element("base_data").Element(elementName);
            if (raceElement != null) {
                BaseData data = new BaseData();
                data.Load(raceElement);
                RaceBaseData[race] = data;
            }
        }

        public BaseData GetBaseData(Race race) {
            if(RaceBaseData.ContainsKey(race)) {
                return RaceBaseData[race];
            }
            return null;
        }

        public bool HasCondition<T>(Race race) where T : QuestCondition {
            return CompleteConditions[race].HasCondition<T>();
        }

        public T GetCondition<T>(Race race) where T : QuestCondition {
            return CompleteConditions[race].GetCondition<T>();
        }

        public string GetHintWorld(Race race) {
            return CompleteConditions[race].HintWorld;
        }
    }

    public class BaseData {
        public string Owner { get; private set; }
        public string StartText { get; private set; }
        public string EndText { get; private set; }

        public void Load(UniXMLElement element) {
            Owner = element.GetString("owner");
            StartText = element.GetString("start_text");
            EndText = element.GetString("final_text");
        }
    }

    public class QuestRewardData {
        public QuestRewardType Type { get; private set; }
        public int Count { get; private set; }
        public IInventoryObjectInfo Item { get; private set; }

        public void Load(UniXMLElement element) {
            Type = element.GetEnum<QuestRewardType>("type");
            Count = element.GetInt("count");
        }
    }
}
