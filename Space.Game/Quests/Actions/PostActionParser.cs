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
                        return new StartQuestPostAction(name, questId);
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
