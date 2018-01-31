/*
using Nebula.Quests.Actions;
using Nebula.Quests.Drop;
using Nebula.Quests.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class QuestData {
        private string m_Id;
        private List<QuestCondition> m_StartConditions;
        private List<QuestCondition> m_CompleteConditions;
        private List<QuestVariableData> m_QuestVariables;
        private List<PostAction> m_PostActions;
        private List<DropInfo> m_DropInfoList;
        private List<QuestTrigger> m_Triggers;


        public QuestData(string id, List<QuestCondition> startConditions, List<QuestCondition> completeConditions, List<QuestVariableData> variables, List<PostAction> postActions, List<DropInfo> diList,
            List<QuestTrigger> triggers ) {
            m_Id = id;
            m_StartConditions = startConditions;
            m_CompleteConditions = completeConditions;
            m_QuestVariables = variables;
            m_PostActions = postActions;
            m_DropInfoList = diList;
            m_Triggers = triggers;
        }

        public string id {
            get {
                return m_Id;
            }
        }

        public List<QuestCondition> startConditions {
            get {
                return m_StartConditions;
            }
        }

        public List<QuestCondition> completeConditions {
            get {
                return m_CompleteConditions;
            }
        }

        public List<QuestVariableData> variables {
            get {
                return m_QuestVariables;
            }
        }

        public List<PostAction> postActions {
            get {
                return m_PostActions;
            }
        }

        public List<DropInfo> dropInfoList {
            get {
                return m_DropInfoList;
            }
        }

        public List<QuestTrigger> triggers {
            get {
                return m_Triggers;
            }
        }

        public List<QuestCondition> FilterCompleteConditions(string name) {
            List<QuestCondition> filtered = new List<QuestCondition>();
            foreach(var condition in completeConditions) {
                if(condition.name == name ) {
                    filtered.Add(condition);
                }
            }
            return filtered;
        }

        public QuestCondition GetVariableValueCompleteCondition(string variableName ) {
            foreach(var c in completeConditions ) {
                if(c is VariableValueCondition ) {
                    if( (c as VariableValueCondition).variableName == variableName ) {
                        return c;
                    }
                }
            }
            return null;
        }

    }
}
*/
