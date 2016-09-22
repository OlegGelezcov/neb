using Nebula.Quests.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public abstract class QuestCondition {

        public QuestCondition(string name) {
            m_Name = name;
            
        }

        //public void SetPostActions(List<PostAction> postActions) {
        //    m_PostActions = postActions;
        //}

        public abstract bool CheckCondition(IQuestConditionTarget target, object data = null);

        public void SetQuest(IQuest quest) {
            m_Quest = quest;
        }

        private IQuest m_Quest;
        

        protected IQuest quest {
            get {
                return m_Quest;
            }
        }

        private string m_Name;

        public string name {
            get {
                return m_Name;
            }
        }

        //private List<PostAction> m_PostActions;

        //protected List<PostAction> postActions {
        //    get {
        //        return m_PostActions;
        //    }
        //}

        //public void ExecutePostActions(IQuestConditionTarget target) {
        //    if(postActions != null ) {
        //        target.ExecutePostActionList(postActions);
        //        target.TryCheckActiveQuests();
        //    }
        //}
    }
}
