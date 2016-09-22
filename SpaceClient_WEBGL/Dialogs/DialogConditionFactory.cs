using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Dialogs {
    public class DialogConditionFactory {

        private DialogCondition Create(UniXMLElement element) {
            string type = element.GetString("type");
            switch(type) {
                case QuestConditionName.ON_STATION: {
                        return new OnStationCondition();
                    }
                case QuestConditionName.QUEST_COMPLETED: {
                        string questId = element.GetString("id");
                        return new QuestCompletedCondition(questId);
                    }
                case QuestConditionName.AT_SPACE: {
                        return new AtSpaceCondition();
                    }
                case QuestConditionName.DIALOG_COMPLETED: {
                        string id = element.GetString("id");
                        return new DialogCompletedCondition(id);
                    }
                case QuestConditionName.USER_EVENT: {
                        UserEventName eventName = (UserEventName)Enum.Parse(typeof(UserEventName), element.GetString("name"));
                        return CreateUserEventCondition(element, eventName);
                    }
                case QuestConditionName.COUNT_OF_ITEMS_GE: {
                        string id = element.GetString("id");
                        InventoryObjectType itemType = (InventoryObjectType)Enum.Parse(typeof(InventoryObjectType), element.GetString("item_type"));
                        int value = element.GetInt("value");
                        string updateText = string.Empty;
                        if (element.HasAttribute("update_text")) {
                            updateText = element.GetString("update_text");
                        }
                        return new CountOfItemsGECondition(itemType, id, value, updateText);
                    }
                case QuestConditionName.INT_VARIABLE_VALUE_EQ:
                case QuestConditionName.INT_VARIABLE_VARLUE_GE:
                case QuestConditionName.FLOAT_VARIABLE_VALUE_EQ:
                case QuestConditionName.BOOL_VARIABLE_VALUE_EQ:
                    return ParseVariableValueCondtion(type, element);
                default:
                    return null;
            }
        }

        private DialogCondition ParseVariableValueCondtion(string name, UniXMLElement element) {
            string varName = element.GetString("name");
            string updateText = string.Empty;
            if(element.HasAttribute("update_text")) {
                updateText = element.GetString("update_text");
            }
            switch(name) {
                case QuestConditionName.INT_VARIABLE_VALUE_EQ: {
                        int val = element.GetInt("value");
                        return new IntVariableValueEQCondition(varName, val, updateText);
                    }
                case QuestConditionName.INT_VARIABLE_VARLUE_GE: {
                        int val = element.GetInt("value");
                        return new IntVariableValueGECondition(varName, val, updateText);
                    }
                case QuestConditionName.BOOL_VARIABLE_VALUE_EQ: {
                        bool val = element.GetBool("value");
                        return new BoolVariableValueEQCondition(varName, val, updateText);
                    }
                case QuestConditionName.FLOAT_VARIABLE_VALUE_EQ: {
                        float val = element.GetFloat("value");
                        return new FloatVariableValueEQCondition(varName, val, updateText);
                    }
                default:
                    return null;
            }
        }

        private DialogCondition CreateUserEventCondition(UniXMLElement element, UserEventName eventName ) {
            return new UserEventCondition(eventName);
        }

        public List<DialogCondition> CreateList(UniXMLElement parent) {
            List<DialogCondition> result = new List<DialogCondition>();
            var dumpList = parent.Elements("condition").Select(cElement => {
                var condition = Create(cElement);
                if (condition != null) {
                    result.Add(condition);
                }
                return condition;
            }).ToList();
            return result;
        }
    }
}
