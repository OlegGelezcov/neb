/*
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class QuestCompletedCondition : QuestCondition {
        private string m_QuestId;

        public QuestCompletedCondition(string qid) : base(QuestConditionName.QUEST_COMPLETED) {
            m_QuestId = qid;
        }


        public override bool CheckCondition(IQuestConditionTarget target, object userEvent = null) {
            return target.isQuestCompleted(m_QuestId);
        }
    }
}
*/
