using Common;
using Nebula.Quests.Actions;
using Nebula.Quests.Drop;
using Nebula.Quests.Triggers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Quests {
    public class QuestDataCollection : ConcurrentDictionary<string, QuestData> {


        public void Load(string file) {
            Clear();
            QuestConditionFactory conditionFactory = new QuestConditionFactory();
            QuestVariableDataFactory variableFactory = new QuestVariableDataFactory();
            QuestTriggerParser triggerParser = new QuestTriggerParser();

            XDocument document = XDocument.Load(file);
            PostActionParser postActionParser = new PostActionParser();
            var dumpLst = document.Element("quests").Elements("quest").Select(questElement => {
                string id = questElement.GetString("id");
                List<QuestCondition> startConditions = conditionFactory.CreateConditions(questElement.Element("start_conditions"));
                List<QuestCondition> completeConditions = conditionFactory.CreateConditions(questElement.Element("complete_conditions"));
                List<QuestVariableData> variables = variableFactory.CreateVariables(questElement.Element("variables"));
                List<QuestTrigger> triggers = triggerParser.ParseList(questElement.Element("triggers"));

                List<PostAction> postActions = null;
                XElement postActionsElement = questElement.Element("post_actions");
                if (postActionsElement != null) {
                    postActions = postActionParser.ParseList(postActionsElement);
                } else {
                    postActions = new List<PostAction>();
                }

                XElement dropItemsElement = questElement.Element("drop_items");
                List<DropInfo> dropInfoList = ParseDropInfoList(dropItemsElement);
                

                QuestData questData = new QuestData(id, startConditions, completeConditions, variables, postActions, dropInfoList, triggers);
                TryAdd(id, questData);
                return questData;
            }).ToList();
        }


        private List<DropInfo> ParseDropInfoList(XElement parent ) {
            if(parent == null ) {
                return new List<DropInfo>();
            }
            var result = parent.Elements("item").Select(itemElement => {
                var dropItem = ParseDropItem(itemElement);
                var dropSource = ParseDropSource(itemElement);
                return new DropInfo(dropItem, dropSource);
            }).ToList();
            return result;
        }

        private DropItem ParseDropItem(XElement element) {
            string id = element.GetString("id");
            InventoryObjectType type = (InventoryObjectType)Enum.Parse(typeof(InventoryObjectType), element.GetString("type"));
            int count = element.GetInt("count");

            switch(type) {
                case InventoryObjectType.quest_item:
                    return new QuestObjectDropItem(id, count, element.GetString("quest"));
                default:
                    return new DropItem(id, type, count);
            }
        }

        private DropSource ParseDropSource(XElement element) {
            ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), element.GetString("source_type"));
            switch(itemType) {
                case ItemType.Asteroid: {
                        return new AsteroidDropSource(element.GetString("asteroid_data_id"));
                    }
                case ItemType.QuestChest: {
                        return new QuestChestDropSource(element.GetString("quest"));
                    }
                default:
                    return null;
            }
        }

        public QuestData GetQuest(string id) {
            QuestData result = null;
            if(TryGetValue(id, out result)) {
                return result;
            }
            return null;
        }
    }
}
