using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Quests {
    public class QuestConditionFactory {

        private QuestCondition Create(XElement xmlElement) {
            string type = xmlElement.GetString("type");
            switch(type) {
                case QuestConditionName.ON_STATION: {
                        return new OnStationCondition();
                    }
                case QuestConditionName.AT_SPACE: {
                        return new OnSpaceCondition();
                    }
                case QuestConditionName.QUEST_COMPLETED: {
                        string questId = xmlElement.GetString("id");
                        return new QuestCompletedCondition(questId);
                    }
                case QuestConditionName.DIALOG_COMPLETED: {
                        string dialogId = xmlElement.GetString("id");
                        return new DialogCompletedCondition(dialogId);
                    }
                case QuestConditionName.USER_EVENT: {
                        UserEventName eventName = (UserEventName)Enum.Parse(typeof(UserEventName), xmlElement.GetString("name"));
                        return CreateUserEventCondition(xmlElement, eventName);
                    }
                case QuestConditionName.INT_VARIABLE_VALUE_EQ: {
                        string varName = xmlElement.GetString("name");
                        int varValuee = xmlElement.GetInt("value");
                        return new IntVariableValueEqCondition(varName, varValuee);
                    }
                case QuestConditionName.FLOAT_VARIABLE_VALUE_EQ: {
                        string varName = xmlElement.GetString("name");
                        float varValue = xmlElement.GetFloat("value");
                        return new FloatVariableValueEqCondition(varName, varValue);
                    }
                case QuestConditionName.BOOL_VARIABLE_VALUE_EQ: {
                        string varName = xmlElement.GetString("name");
                        bool varValue = xmlElement.GetBool("value");
                        return new BoolVariableValueEqCondition(varName, varValue);
                    }
                default: {
                        return new EmptyQuestCondition();
                    }
            }
        }

        private QuestCondition CreateUserEventCondition(XElement xmlElement, UserEventName eventName ) {
            switch(eventName) {
                case UserEventName.object_scanner_select_ship:
                case UserEventName.start_moving:
                case UserEventName.rotate_camera: {
                        return new UserEventNameCondition(eventName);
                    }
                default: {
                        return new EmptyQuestCondition();
                    }
            }
        }

        public List<QuestCondition> CreateConditions(XElement parentElement) {
            List<QuestCondition> conditions = parentElement.Elements("condition").Select(element => {
                return Create(element);
            }).ToList();
            return conditions;
        }
    }


}
