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
