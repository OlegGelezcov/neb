using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Nebula.Client.Dialogs {
    public class DialogContainer : Dictionary<string, Dialog>{

        public DialogContainer() {
            
        }

        public void Load(string xml) {
            Clear();
            DialogConditionFactory conditionFactory = new DialogConditionFactory();
            UniXmlDocument document = new UniXmlDocument(xml);
            var dumpList = document.document.Element("dialogs").Elements("dialog").Select(dElem => {
                string id = dElem.GetString("id");
                var topicList = dElem.Element("topics").Elements("topic").Select(tElem => {
                    string character = tElem.GetString("character");
                    string voice = tElem.GetString("voice");
                    string text = tElem.GetString("text");
                    return new DialogTopic(character, text, voice);
                }).ToList();

                List<DialogCondition> startConditions = null;
                var startConditionsElement = dElem.Element("start_conditions");
                if(startConditionsElement != null ) {
                    startConditions = conditionFactory.CreateList(new UniXMLElement(startConditionsElement));
                }
                if(startConditions == null ) {
                    startConditions = new List<DialogCondition>();
                }

                int index = dElem.GetInt("index");

                Dialog dialog = new Dialog(id, topicList, startConditions, index);
                Add(id, dialog);
                return dialog;
            }).ToList();
        }

        public DialogContainer(Dictionary<string, Dialog> dialogs ) {
            foreach(var kvp in dialogs) {
                Add(kvp.Key, kvp.Value);
            }
        }

        public Dialog GetDialog(string id) {
            if(ContainsKey(id)) {
                return this[id];
            }
            return null;
        }

        public Dialog GetMinIndexDialog(List<string> excludes) {
            if(excludes == null ) {
                excludes = new List<string>();
            }

            int minIndex = int.MaxValue;
            Dialog target = null;
            foreach(Dialog dlg in Values ) {
                if(dlg.index < minIndex ) {
                    if(!excludes.Contains(dlg.id)) {
                        minIndex = dlg.index;
                        target = dlg;
                    }
                }
            }
            return target;
        }

    }
}
