using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Dialogs {
    public class QuestCompletedCondition : DialogCondition {

        private string m_QuestId;

        public QuestCompletedCondition(string questId ) {
            m_QuestId = questId;
        }
        public override bool CheckCondition(IDialogConditionTarget target) {
            return target.IsQuestCompleted(m_QuestId);
        }
    }
}
