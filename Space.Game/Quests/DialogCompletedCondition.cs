/*
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class DialogCompletedCondition : QuestCondition {
        private string m_DialogId;

        public DialogCompletedCondition(string dlgid ) : 
            base(QuestConditionName.DIALOG_COMPLETED) {
            m_DialogId = dlgid;
        }

        public override bool CheckCondition(IQuestConditionTarget target, object data = null) {
            return target.isDialogCompleted(m_DialogId);
        }
    }
}
*/