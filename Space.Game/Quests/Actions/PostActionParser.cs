/*
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Quests.Actions {
    public class PostActionParser {

        private PostAction Parse(XElement element) {
            string name = element.GetString("name");
            switch(name) {
                case PostActionName.START_QUEST: {
                        string questId = element.GetString("id");
                        return new StartQuestPostAction(questId);
                    }
                case PostActionName.REMOVE_ITEM: {
                        InventoryObjectType type = (InventoryObjectType)Enum.Parse(typeof(InventoryObjectType), element.GetString("type"));
                        string id = element.GetString("id");
                        int count = element.GetInt("count");
                        return new RemoveItemPostAction(type, id, count);
                    }
                case PostActionName.ADD_ITEM_TO_HANGAR_UNIQUE: {
                        InventoryObjectType type = (InventoryObjectType)Enum.Parse(typeof(InventoryObjectType), element.GetString("type"));
                        string id = element.GetString("id");
                        int count = element.GetInt("count");
                        string quest = element.GetString("quest");
                        return new AddItemToHangarUniquePostAction(type, id, count, quest);
                    }
                case PostActionName.SET_BOOL_VARIABLE: {
                        string varname = element.GetString("var_name");
                        bool varvalue = element.GetBool("var_value");
                        return new SetBoolVariablePostAction(varname, varvalue);
                    }
                default: {
                        return null;
                    }
            }
        }

        public List<PostAction> ParseList(XElement parent) {
            List<PostAction> result = new List<PostAction>();
            var dumpList = parent.Elements("action").Select(element => {
                var action = Parse(element);
                if(action != null ) {
                    result.Add(action);
                }
                return action;
            }).ToList();
            return result;
        }
    }
}
*/