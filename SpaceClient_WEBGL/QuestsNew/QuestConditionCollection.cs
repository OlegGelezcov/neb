using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Quests {
    public class QuestConditionCollection {
        public List<QuestCondition> Conditions { get; } = new List<QuestCondition>();

        public int Repeat { get; private set; } = 1;
        public string HintWorld { get; private set; } = string.Empty;

        public void Load(UniXMLElement parent) {
            Repeat = parent.GetInt("repeat", 1);
            HintWorld = parent.GetString("hint_world", string.Empty);

            Conditions.Clear();
            QuestConditionParser parser = new QuestConditionParser();
            foreach(UniXMLElement conditionElement in parent.Elements("condition")) {
                QuestCondition condition = parser.Parse(conditionElement);
                if(condition != null ) {
                    Conditions.Add(condition);
                }
            }
        }

        public bool HasCondition<T>() where T : QuestCondition {
            foreach(var condition in Conditions) {
                if(condition is T ) {
                    return true;
                }
            }
            return false;
        }

        public T GetCondition<T>() where T : QuestCondition {
            foreach (var condition in Conditions) {
                if (condition is T) {
                    return (condition as T);
                }
            }
            return default(T);
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Repeat => {Repeat}, Hint World => {HintWorld}");
            Conditions.ForEach(c => {
                builder.AppendLine(c.ToString());
            });
            return builder.ToString();
        }
    }
}
