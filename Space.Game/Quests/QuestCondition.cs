using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public abstract class QuestCondition {
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
    }
}
