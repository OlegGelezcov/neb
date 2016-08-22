using Nebula.Quests.Actions;
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


        public QuestData(string id, List<QuestCondition> startConditions, List<QuestCondition> completeConditions, List<QuestVariableData> variables, List<PostAction> postActions) {
            m_Id = id;
            m_StartConditions = startConditions;
            m_CompleteConditions = completeConditions;
            m_QuestVariables = variables;
            m_PostActions = postActions;
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
    }
}
