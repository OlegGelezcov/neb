using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Quests {
    public class QuestData {
        public string Id { get; private set; }
        public QuestType Type { get; private set; }
        public QuestConditionCollection StartConditions { get; } = new QuestConditionCollection();

        public ConcurrentDictionary<Race, QuestConditionCollection> CompleteConditions { get; } =
            new ConcurrentDictionary<Race, QuestConditionCollection>();

        public QuestRewardCollection Rewards { get; } = new QuestRewardCollection();

        public bool TryGetCompleteConditions(Race race, out QuestConditionCollection conditions ) {
            conditions = null;
            if(CompleteConditions.TryGetValue(race, out conditions)) {
                return true;
            }
            return false;
        }

        public bool TryGetCompleteCondition<T>(Race race, out T condition) where T : QuestCondition {
            condition = default(T);
            QuestConditionCollection conditionCollection;
            if(TryGetCompleteConditions(race, out conditionCollection)) {
                if(conditionCollection.HasCondition<T>()) {
                    condition = conditionCollection.GetCondition<T>();
                    return true;
                }
            }
            return false;
        }

        public override string ToString() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Quest => {Id}, Type => {Type}");
            stringBuilder.AppendLine("Start conditions => ");
            stringBuilder.AppendLine(StartConditions.ToString());
            stringBuilder.AppendLine("Complete conditions => ");
            stringBuilder.AppendLine("Humans    =>");
            stringBuilder.AppendLine(CompleteConditions[Race.Humans].ToString());
            stringBuilder.AppendLine("Borgs     =>");
            stringBuilder.AppendLine(CompleteConditions[Race.Borguzands].ToString());
            stringBuilder.AppendLine("Krips     =>");
            stringBuilder.AppendLine(CompleteConditions[Race.Criptizoids].ToString());
            stringBuilder.AppendLine("Reward    =>");
            stringBuilder.AppendLine(Rewards.ToString());
            return stringBuilder.ToString();
        }

        public void Load(XElement element ) {
            Id = element.GetString("id");
            Type = element.GetEnum<QuestType>("type");

            var startConditionsElement = element.Element("start_conditions");
            StartConditions.Load(startConditionsElement);

            var humansCompleteConditions = element.Element("complete_conditions").Element("humans");
            var borgsCompleteConditions = element.Element("complete_conditions").Element("borgs");
            var kripsCompleteConditions = element.Element("complete_conditions").Element("krips");

            QuestConditionCollection humanComplete = new QuestConditionCollection();
            humanComplete.Load(humansCompleteConditions);

            QuestConditionCollection borgsComple = new QuestConditionCollection();
            borgsComple.Load(borgsCompleteConditions);

            QuestConditionCollection kripsComplete = new QuestConditionCollection();
            kripsComplete.Load(kripsCompleteConditions);

            CompleteConditions.TryAdd(Race.Humans, humanComplete);
            CompleteConditions.TryAdd(Race.Borguzands, borgsComple);
            CompleteConditions.TryAdd(Race.Criptizoids, kripsComplete);

            var rewardsElement = element.Element("rewards");
            if(rewardsElement != null ) {
                Rewards.Load(rewardsElement);
            }
        }
    }
}
