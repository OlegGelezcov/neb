/*
using Common;
using GameMath;
using Nebula.Quests.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Quests {
    public class QuestConditionFactory {

        private readonly PostActionParser m_ActionParser = new PostActionParser();

        private QuestCondition Create(XElement xmlElement) {
            string type = xmlElement.GetString("type");

            //var postActions = GetPostActionsFromConditionElement(xmlElement);

            QuestCondition condition = null;
            switch(type) {
                case QuestConditionName.ON_STATION: {
                        condition = new OnStationCondition();
                    }
                    break;
                case QuestConditionName.AT_SPACE: {
                        condition = new OnSpaceCondition();
                    }
                    break;
                case QuestConditionName.QUEST_COMPLETED: {
                        string questId = xmlElement.GetString("id");
                        condition = new QuestCompletedCondition(questId);
                    }
                    break;
                case QuestConditionName.DIALOG_COMPLETED: {
                        string dialogId = xmlElement.GetString("id");
                        condition = new DialogCompletedCondition(dialogId);
                    }
                    break;
                case QuestConditionName.IS_WORLD: {
                        string worldId = xmlElement.GetString("id");
                        condition = new IsWorldCondition(worldId);
                    }
                    break;
                case QuestConditionName.USER_EVENT: {
                        UserEventName eventName = (UserEventName)Enum.Parse(typeof(UserEventName), xmlElement.GetString("name"));
                        condition = CreateUserEventCondition(xmlElement, eventName);
                    }
                    break;
                case QuestConditionName.INT_VARIABLE_VALUE_EQ: {
                        string varName = xmlElement.GetString("name");
                        int varValuee = xmlElement.GetInt("value");
                        condition = new IntVariableValueEqCondition(varName, varValuee);
                    }
                    break;
                case QuestConditionName.FLOAT_VARIABLE_VALUE_EQ: {
                        string varName = xmlElement.GetString("name");
                        float varValue = xmlElement.GetFloat("value");
                        condition = new FloatVariableValueEqCondition(varName, varValue);
                    }
                    break;
                case QuestConditionName.BOOL_VARIABLE_VALUE_EQ: {
                        string varName = xmlElement.GetString("name");
                        bool varValue = xmlElement.GetBool("value");
                        condition = new BoolVariableValueEqCondition(varName, varValue);
                    }
                    break;
                case QuestConditionName.INT_VARIABLE_VARLUE_GE: {
                        string varName = xmlElement.GetString("name");
                        int varValuee = xmlElement.GetInt("value");
                        condition = new IntVariableValueGECondition(varName, varValuee);
                    }
                    break;
                case QuestConditionName.COUNT_OF_ITEMS_GE: {
                        string id = xmlElement.GetString("id");
                        InventoryObjectType itType = (InventoryObjectType)Enum.Parse(typeof(InventoryObjectType), xmlElement.GetString("item_type"));
                        int value = xmlElement.GetInt("value");
                        condition = new CountOfItemsGECondition(itType, id, value);
                    }
                    break;
                case QuestConditionName.QUEST_ITEM_USED_NEAR_ACTIVATOR_WITH_BADGE: {
                        string id = xmlElement.GetString("id");
                        string badge = xmlElement.GetString("badge");
                        condition = new QuestItemUsedNearActivatorWithBadgeCondition(id, badge);
                    }
                    break;
                case QuestConditionName.QUEST_ITEM_USED_NEAR_POINT: {
                        string id = xmlElement.GetString("id");
                        string world = xmlElement.GetString("world");
                        Vector3 point = xmlElement.GetFloatArray("point").ToVector3();
                        float radius = xmlElement.GetFloat("radius");
                        condition = new QuestItemUsedNearPointCondition(id, world, radius, point);
                    }
                    break;
                default: {
                        condition = new EmptyQuestCondition();
                    }
                    break;
            }
            //if(condition != null ) {
            //    condition.SetPostActions(postActions);
            //}
            return condition;
        }

        private List<PostAction> GetPostActionsFromConditionElement(XElement conditionElement) {
            var parent = conditionElement.Element("post_actions");
            if(parent != null ) {
                return m_ActionParser.ParseList(parent);
            }
            return null;
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
*/

