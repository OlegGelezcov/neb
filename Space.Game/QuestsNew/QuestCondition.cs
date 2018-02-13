using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Quests {
    public abstract class QuestCondition {
        public abstract QuestConditionType Type { get; }
        public abstract bool Check(IQuestConditionContext context);
        public virtual bool IsClearVariable { get; } = false;
        public virtual string VariableName { get; } = "";
        public virtual void ResetVariable(IQuestConditionContext context) { }
    }

    

    public class PlayerLevelGEQuestCondition : QuestCondition {

        public int Level { get; private set; }

        public PlayerLevelGEQuestCondition(XElement element) {
            Level = element.GetInt("value");
        }

        public override QuestConditionType Type => QuestConditionType.player_level_ge;

        public override bool Check(IQuestConditionContext context) {
            return (context.PlayerLevel >= Level);
        }

        public override string ToString() {
            return $"{GetType().Name} => {Level}";
        }
    }

    public class NpcKilledWithBotGroupQuestCondition : QuestCondition {
        public string BotGroup { get; private set; }

        public NpcKilledWithBotGroupQuestCondition(XElement element) {
            BotGroup = element.GetString("value");
        }

        public override QuestConditionType Type => QuestConditionType.npc_killed_with_bot_group;

        public override bool Check(IQuestConditionContext context) {
            if(context.KilledNpc == null ) {
                return false;
            }
            return (context.KilledNpc.BotGroup == BotGroup);
        }

        public override string ToString() {
            return $"{GetType().Name} => {BotGroup}";
        }
    }

    public class NpcKilledWithLevelQuestCondition : QuestCondition {
        public int Level { get; private set; }

        public NpcKilledWithLevelQuestCondition(XElement element) {
            Level = element.GetInt("value");
        }

        public override QuestConditionType Type => QuestConditionType.npc_killed_with_level;

        public override bool Check(IQuestConditionContext context) {
            if(context.KilledNpc == null ) {
                return false;
            }
            return (context.KilledNpc.Level >= Level);
        }

        public override string ToString() {
            return $"{GetType().Name} => {Level}";
        }
    }

    public class NpcKilledWithClassQuestCondition : QuestCondition {

        public List<Workshop> Classes { get; private set; }

        public override QuestConditionType Type => QuestConditionType.npc_killed_with_class;

        public NpcKilledWithClassQuestCondition(XElement element) {
            Classes = new List<Workshop>();
            string classStr = element.GetString("value");
            if(string.IsNullOrEmpty(classStr) || (classStr.ToLower() == "any")) {
                Classes.AddRange((Workshop[])Enum.GetValues(typeof(Workshop)));
            } else {
                string[] classesArr = classStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string str in classesArr) {
                    Workshop workshop;
                    if(Enum.TryParse<Workshop>(str, out workshop)) {
                        Classes.Add(workshop);
                    }
                }
            }
        }

        public override bool Check(IQuestConditionContext context) {
            if(context.KilledNpc == null ) {
                return false;
            }
            return Classes.Contains(context.KilledNpc.Class);
        }

        public override string ToString() {
            return $"{GetType().Name} => {string.Join(",", Classes)}";
        }
    }

    public class NpcKilledWithColorQuestCondition : QuestCondition {

        public List<ObjectColor> Colors { get; private set; }

        public override QuestConditionType Type => QuestConditionType.npc_killed_with_color;

        public NpcKilledWithColorQuestCondition(XElement element) {
            Colors = new List<ObjectColor>();
            string colorStr = element.GetString("value");
            if(string.IsNullOrEmpty(colorStr) || colorStr.ToLower() == "any") {
                Colors.AddRange((ObjectColor[])Enum.GetValues(typeof(ObjectColor)));
            } else {
                string[] colorArr = colorStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string c in colorArr) {
                    ObjectColor color;
                    if(Enum.TryParse(c, out color)) {
                        Colors.Add(color);
                    }
                }
            }
        }

        public override bool Check(IQuestConditionContext context) {
            if(context.KilledNpc == null ) {
                return false;
            }
            return Colors.Contains(context.KilledNpc.Color);
        }

        public override string ToString() {
            return $"{GetType().Name} => {string.Join(",", Colors)}";
        }
    }

    public class QuestCompletedQuestCondition : QuestCondition {
        public List<string> QuestIds { get; private set; }

        public QuestCompletedQuestCondition(XElement element) {
            string questStr = element.GetString("value");
            string[] questArr = questStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            QuestIds = new List<string>();
            QuestIds.AddRange(questArr);
        }

        public override QuestConditionType Type => QuestConditionType.quest_completed;

        public override bool Check(IQuestConditionContext context) {
            return QuestIds.All(qid => context.IsQuestCompleted(qid));
        }

        public override string ToString() {
            return $"{GetType().Name} => {string.Join(",", QuestIds)}";
        }
    }

    public class ModuleCraftedQuestCondition : QuestCondition {
        public ShipModelSlotType ModuleType { get; private set; }
        public ObjectColor Color { get; private set; }
        public int Level { get; private set; }

        public ModuleCraftedQuestCondition(XElement element) {
            ModuleType = element.GetEnum<ShipModelSlotType>("value");
            Color = element.GetEnum<ObjectColor>("color");
            Level = element.GetInt("level");
        }

        public override QuestConditionType Type => QuestConditionType.module_crafted;

        public override bool Check(IQuestConditionContext context) {
            if(context.CraftedModule == null ) {
                return false;
            }
            return (context.CraftedModule.Color == Color) && (context.CraftedModule.Level >= Level) && (context.CraftedModule.Slot == ModuleType);
        }

        //public override string VariableName => ModuleType.ToString() + Color.ToString() + Level.ToString();

        //public override bool IsClearVariable => true;

        //public override void ResetVariable(IQuestConditionContext context) {
        //    context.ResetVariable<bool>(VariableName);
        //}

        public override string ToString() {
            return $"{GetType().Name} => {ModuleType}:{Color}";
        }
    }

    public class CollectOreQuestCondition : QuestCondition {
        public string OreId { get; private set; }
        public int Count { get; private set; }

        public override QuestConditionType Type => QuestConditionType.collect_ore;

        public CollectOreQuestCondition(XElement element) {
            OreId = element.GetString("value");
            Count = element.GetInt("count");
        }

        public override bool Check(IQuestConditionContext context) {
            return context.GetVariable<int>(OreId) >= Count;
        }

        public override bool IsClearVariable => true;

        public override string VariableName => OreId;

        public override void ResetVariable(IQuestConditionContext context) {
            context.ResetVariable<int>(VariableName);
        }

        public override string ToString() {
            return $"{GetType().Name} => {OreId}, Count => {Count}";
        }
    }

    public class CreateStructureQuestCondition : QuestCondition {

        public QuestStructureType Structure { get; private set; }
        //public int Count { get; private set; }

        public CreateStructureQuestCondition(XElement element) {
            Structure = element.GetEnum<QuestStructureType>("value");
            //Count = element.GetInt("count");
        }

        public override QuestConditionType Type => QuestConditionType.create_structure;

        public override bool Check(IQuestConditionContext context) {
            if(context.CreatedStructure == null ) {
                return false;
            }
            return context.CreatedStructure.Type == Structure;
        }

        //public override bool IsClearVariable => true;

        //public override string VariableName => Structure.ToString();

        //public override void ResetVariable(IQuestConditionContext context) {
        //    context.ResetVariable<int>(VariableName);
        //}

        public override string ToString() {
            return $"{GetType().Name} => {Structure}";
        }
    }

    public class ReachLevelQuestCondition : QuestCondition {

        public int Level { get; private set; }

        public ReachLevelQuestCondition(XElement element) {
            Level = element.GetInt("value");
        }

        public override QuestConditionType Type => QuestConditionType.level_reached;

        public override bool Check(IQuestConditionContext context) {
            return (context.PlayerLevel >= Level);
        }

        public override string ToString() {
            return $"{GetType().Name} => {Level}";
        }
    }

    public class CompanionCreatedQuestCondition : QuestCondition {

        public override QuestConditionType Type => QuestConditionType.create_companion;

        public override bool Check(IQuestConditionContext context) {
            return context.GetVariable<bool>(VariableName);
        }

        public CompanionCreatedQuestCondition(XElement element) { }

        public override bool IsClearVariable => true;
        public override string VariableName => "companion";

        public override void ResetVariable(IQuestConditionContext context) {
            context.ResetVariable<bool>("companion");
        }

        public override string ToString() {
            return $"{GetType().Name}";
        }
    }

    public class KillPlayerQuestCondition : QuestCondition {

        public List<Race> Races { get; }

        public override QuestConditionType Type => throw new NotImplementedException();

        public KillPlayerQuestCondition(XElement element) {
            string strVal = element.GetString("value");

            Races = new List<Race>();
            if(string.IsNullOrEmpty(strVal) || strVal.ToLower() == "any" ) {
                Races.AddRange((Race[])Enum.GetValues(typeof(Race)));
            } else {
                string[] arr = strVal.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string sr in arr) {
                    Race race;
                    if(Enum.TryParse<Race>(sr, out race)) {
                        Races.Add(race);
                    }
                }
            }
        }

        public override bool Check(IQuestConditionContext context) {
            if(context.KilledPlayer == null ) {
                return false;
            }
            return Races.Contains(context.KilledPlayer.Race);
        }

        public override string ToString() {
            return $"{GetType().Name} => {string.Join(",", Races)}";
        }
    }

    public class CaptureSystemQuestCondition : QuestCondition {
        public string System { get; }

        public CaptureSystemQuestCondition(XElement element) {
            System = element.GetString("value");
        }

        public override QuestConditionType Type => QuestConditionType.system_captured;

        public override bool Check(IQuestConditionContext context) {
            if(string.IsNullOrEmpty(System) || (System.ToLower() == "any")) {
                return true;
            }
            return context.CapturedSystem == System;
        }

        public override string ToString() {
            return $"{GetType().Name} => {System}";
        }
    }
}
