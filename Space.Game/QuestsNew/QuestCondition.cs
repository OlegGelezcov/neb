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
            return (context.KilledNpc.Level == Level);
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

    }
}
