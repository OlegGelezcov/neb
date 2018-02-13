using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Quests {
    public class QuestConditionCollection {

        public List<QuestCondition> Conditions { get; } = new List<QuestCondition>();

        public int Repeat { get; private set; } = 1;

        public void Load(XElement parent) {
            Repeat = parent.GetInt("repeat", 1);
            Conditions.Clear();
            QuestConditionParser parser = new QuestConditionParser();
            foreach(XElement conditionElement in parent.Elements("condition")) {
                var condition = parser.Parse(conditionElement);
                Conditions.Add(condition);
            }

            
        }

        public void ResetVariables(IQuestConditionContext context) {
            Conditions.ForEach(c => c.ResetVariable(context));
        }

        public bool HasCondition<T>() where T : QuestCondition {
            foreach(var condition in Conditions) {
                if(condition is T) {
                    return true;
                }
            }
            return false;
        }

        public T GetCondition<T>() where T : QuestCondition {
            foreach(var condition in Conditions ) {
                if(condition is T) {
                    return (condition as T);
                }
            }
            return default(T);
        }

        public bool Check(IQuestConditionContext context) {
            return Conditions.All(c => c.Check(context));
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Repeat => {Repeat}");
            Conditions.ForEach(c => {
                builder.AppendLine(c.ToString());
            });
            return builder.ToString();
        }
    }

    public class QuestConditionParser {

        public QuestCondition Parse(XElement element) {
            QuestConditionType type = element.GetEnum<QuestConditionType>("type");
            switch(type) {
                case QuestConditionType.player_level_ge: {
                        return new PlayerLevelGEQuestCondition(element);
                    }
                case QuestConditionType.npc_killed_with_level: {
                        return new NpcKilledWithLevelQuestCondition(element);
                    }
                case QuestConditionType.npc_killed_with_class: {
                        return new NpcKilledWithClassQuestCondition(element);
                    }
                case QuestConditionType.npc_killed_with_color: {
                        return new NpcKilledWithColorQuestCondition(element);
                    }
                case QuestConditionType.quest_completed: {
                        return new QuestCompletedQuestCondition(element);
                    }
                case QuestConditionType.module_crafted: {
                        return new ModuleCraftedQuestCondition(element);
                    }
                case QuestConditionType.collect_ore: {
                        return new CollectOreQuestCondition(element);
                    }
                case QuestConditionType.create_structure: {
                        return new CreateStructureQuestCondition(element);
                    }
                case QuestConditionType.level_reached: {
                        return new ReachLevelQuestCondition(element);
                    }
                case QuestConditionType.create_companion: {
                        return new CompanionCreatedQuestCondition(element);
                    }
                case QuestConditionType.player_killed: {
                        return new KillPlayerQuestCondition(element);
                    }
                case QuestConditionType.system_captured: {
                        return new CaptureSystemQuestCondition(element);
                    }
                case QuestConditionType.npc_killed_with_bot_group: {
                        return new NpcKilledWithBotGroupQuestCondition(element);
                    }
                default: {
                        throw new NotImplementedException();
                    }
            }
        }
    }
}
