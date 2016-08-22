using Common;
using Nebula.Quests.Actions;
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

            XDocument document = XDocument.Load(file);
            PostActionParser postActionParser = new PostActionParser();
            var dumpLst = document.Element("quests").Elements("quest").Select(questElement => {
                string id = questElement.GetString("id");
                List<QuestCondition> startConditions = conditionFactory.CreateConditions(questElement.Element("start_conditions"));
                List<QuestCondition> completeConditions = conditionFactory.CreateConditions(questElement.Element("complete_conditions"));
                List<QuestVariableData> variables = variableFactory.CreateVariables(questElement.Element("variables"));

                List<PostAction> postActions = null;
                XElement postActionsElement = questElement.Element("post_actions");
                if (postActionsElement != null) {
                    postActions = postActionParser.ParseList(postActionsElement);
                } else {
                    postActions = new List<PostAction>();
                }

                QuestData questData = new QuestData(id, startConditions, completeConditions, variables, postActions);
                TryAdd(id, questData);
                return questData;
            }).ToList();
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
