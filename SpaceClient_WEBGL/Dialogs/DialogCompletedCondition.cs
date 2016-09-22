using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Dialogs {
    public class DialogCompletedCondition : DialogCondition {
        private string m_Id;

        private string id {
            get {
                return m_Id;
            }
        }

        public DialogCompletedCondition(string dialogId ) : base(QuestConditionName.DIALOG_COMPLETED) {
            m_Id = dialogId;
        }

        public override bool CheckCondition(IDialogConditionTarget target) {
            return target.IsDialogCompleted(id);
        }
    }
}
