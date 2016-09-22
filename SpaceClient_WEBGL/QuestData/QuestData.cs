using Common;
using Nebula.Client.Dialogs;
using Nebula.Client.Quests.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.QuestData {
    public class QuestData {
        public string id { get; private set; }
        public string text { get; private set; }
        public List<string> dialogs { get; private set; }
        public string character { get; private set; }
        public List<DialogCondition> completeConditions { get; private set; }
        public List<StageMarker> markers { get; private set; }

        public QuestData(string id, string text, List<string> dialogs, string character, List<DialogCondition> completeConditions, List<StageMarker> markers) {
            this.id = id;
            this.text = text;
            this.dialogs = dialogs;
            this.character = character;
            this.completeConditions = completeConditions;
            this.markers = markers;
        }

        public bool hasCharacter {
            get {
                return (false == string.IsNullOrEmpty(character));
            }
        }

        public bool DependFromUserEvent(UserEventName eventName) {
            if(completeConditions != null ) {
                foreach(var condition in completeConditions ) {
                    if(condition is UserEventCondition ) {
                        if((condition as UserEventCondition).name == eventName ) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public List<DialogCondition> GetConditions(string name) {
            List<DialogCondition> filtered = new List<DialogCondition>();
            foreach(var condition in completeConditions ) {
                if(condition.name == name ) {
                    filtered.Add(condition);
                }
            }
            return filtered;
        }

        public DialogCondition GetVariableValueCondition(string varName ) {
            foreach(var condition in completeConditions ) {
                if(condition is VariableValueCondition ) {
                    if((condition as VariableValueCondition).variableName == varName ) {
                        return condition;
                    }
                }
            }
            return null;
        }
    }
}
