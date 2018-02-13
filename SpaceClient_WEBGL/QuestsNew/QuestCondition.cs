using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Quests {
    public abstract class QuestCondition {
        public abstract QuestConditionType Type { get; }
        public virtual string VariableName { get; } = "";
    }

    public class PlayerLevelGEQuestCondition : QuestCondition {
        public int Level { get; private set; }

        public PlayerLevelGEQuestCondition(UniXMLElement element) {
            Level = element.GetInt("value");
        }

        public override QuestConditionType Type {
            get {
                return QuestConditionType.player_level_ge;
            }
        }

        public override string ToString() {
            return $"{GetType().Name} => {Level}";
        }
    }

    public class NpcKilledWithBotGroupQuestCondition : QuestCondition {
        public string BotGroup { get; private set; } = string.Empty;

        public NpcKilledWithBotGroupQuestCondition(UniXMLElement element) {
            BotGroup = element.GetString("value");
        }

        public override QuestConditionType Type => QuestConditionType.npc_killed_with_bot_group;

        public override string ToString() {
            return $"{GetType().Name} => {BotGroup}";
        }
    }

    public class NpcKilledWithLevelQuestCondition : QuestCondition {
        public int Level { get; private set; }

        public NpcKilledWithLevelQuestCondition(UniXMLElement element) {
            Level = element.GetInt("value");
        }

        public override QuestConditionType Type => QuestConditionType.npc_killed_with_level;

        public override string ToString() {
            return $"{GetType().Name} => {Level}";
        }
    }

    public class NpcKilledWithClassQuestCondition : QuestCondition {
        public List<Workshop> Classes { get; private set; }

        public override QuestConditionType Type => QuestConditionType.npc_killed_with_class;

        public NpcKilledWithClassQuestCondition(UniXMLElement element) {
            Classes = new List<Workshop>();
            string classStr = element.GetString("value");
            if(string.IsNullOrEmpty(classStr) || (classStr.ToLower() == "any")) {
                Classes.AddRange((Workshop[])Enum.GetValues(typeof(Workshop)));
            } else {
                string[] classesArr = classStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in classesArr) {
                    Workshop workshop = (Workshop)Enum.Parse(typeof(Workshop), str);
                    Classes.Add(workshop);
                }
            }
        }

        public override string ToString() {
            return $"{GetType().Name} => {string.Join(",", Classes.Select(c => c.ToString()).ToArray())}";
        }
    }

    public class NpcKilledWithColorQuestCondition : QuestCondition {

        public List<ObjectColor> Colors { get; private set; }

        public override QuestConditionType Type => QuestConditionType.npc_killed_with_color;

        public NpcKilledWithColorQuestCondition(UniXMLElement element) {
            Colors = new List<ObjectColor>();
            string colorStr = element.GetString("value");
            if (string.IsNullOrEmpty(colorStr) || colorStr.ToLower() == "any") {
                Colors.AddRange((ObjectColor[])Enum.GetValues(typeof(ObjectColor)));
            } else {
                string[] colorArr = colorStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string c in colorArr) {
                    ObjectColor color = (ObjectColor)Enum.Parse(typeof(ObjectColor), c);
                    Colors.Add(color);
                }
            }
        }

        public override string ToString() {
            return $"{GetType().Name} => {string.Join(",", Colors.Select(s => s.ToString()).ToArray())}";
        }
    }

    public class QuestCompletedQuestCondition : QuestCondition {
        public List<string> QuestIds { get; private set; }

        public QuestCompletedQuestCondition(UniXMLElement element) {
            string questStr = element.GetString("value");
            string[] questArr = questStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            QuestIds = new List<string>();
            QuestIds.AddRange(questArr);
        }

        public override QuestConditionType Type => QuestConditionType.quest_completed;

        public override string ToString() {
            return $"{GetType().Name} => {string.Join(",", QuestIds.ToArray())}";
        }
    }

    public class ModuleCraftedQuestCondition : QuestCondition {
        public ShipModelSlotType ModuleType { get; private set; }
        public ObjectColor Color { get; private set; }
        public int Level { get; private set; }

        public ModuleCraftedQuestCondition(UniXMLElement element) {
            ModuleType = element.GetEnum<ShipModelSlotType>("value");
            Color = element.GetEnum<ObjectColor>("color");
            Level = element.GetInt("level");
        }

        public override QuestConditionType Type => QuestConditionType.module_crafted;

        public override string ToString() {
            return $"{GetType().Name} => {ModuleType}:{Color}";
        }
    }

    public class CollectOreQuestCondition : QuestCondition {
        public string OreId { get; private set; }
        public int Count { get; private set; }

        public override QuestConditionType Type => QuestConditionType.collect_ore;

        public CollectOreQuestCondition(UniXMLElement element) {
            OreId = element.GetString("value");
            Count = element.GetInt("count");
        }

        public override string VariableName => OreId;

        public override string ToString() {
            return $"{GetType().Name} => {OreId}, Count => {Count}";
        }
    }

    public class CreateStructureQuestCondition : QuestCondition {
        public QuestStructureType Structure { get; private set; }

        public CreateStructureQuestCondition(UniXMLElement element) {
            Structure = element.GetEnum<QuestStructureType>("value");
            //Count = element.GetInt("count");
        }

        public override QuestConditionType Type => QuestConditionType.create_structure;

        public override string ToString() {
            return $"{GetType().Name} => {Structure}";
        }
    }

    public class ReachLevelQuestCondition : QuestCondition {
        public int Level { get; private set; }

        public ReachLevelQuestCondition(UniXMLElement element) {
            Level = element.GetInt("value");
        }

        public override QuestConditionType Type => QuestConditionType.level_reached;

        public override string ToString() {
            return $"{GetType().Name} => {Level}";
        }
    }
}
