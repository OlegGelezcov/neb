using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Quests {
    public class QuestConditionParser {
        public QuestCondition Parse(UniXMLElement element) {
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
                case QuestConditionType.npc_killed_with_bot_group: {
                        return new NpcKilledWithBotGroupQuestCondition(element);
                    }
            }
            return null;
        }
    }
}
