﻿using Common;
using Nebula.Client.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.QuestData {
    public class QuestDataContainer : Dictionary<string, QuestData> {

        public QuestDataContainer() { }

        public void Load(string xml) {
            Clear();
            UniXmlDocument document = new UniXmlDocument(xml);
            DialogConditionFactory conditionFactory = new DialogConditionFactory();

            var dumpList = document.document.Element("quests").Elements("quest").Select(questElement => {
                string id = questElement.GetString("id");
                string text = questElement.GetString("task_text");
                string character = questElement.GetString("character");

                List<string> dialogs = questElement.GetString("dialogs").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                List<DialogCondition> conditionList = null;
                var completeCondElement = questElement.Element("complete_conditions");
                if(completeCondElement != null ) {
                    conditionList = conditionFactory.CreateList(new UniXMLElement(completeCondElement));
                } else {
                    conditionList = new List<DialogCondition>();
                }
                
                QuestData questData = new QuestData(id, text, dialogs, character, conditionList);
                Add(id, questData);
                return questData;
            }).ToList();
        }

        public QuestData GetQuest(string id) {
            if(ContainsKey(id)) {
                return this[id];
            }
            return null;
        }
    }
}