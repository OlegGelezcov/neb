using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Dialogs {
    public abstract class DialogCondition {

        public DialogCondition(string name) {
            m_Name = name;
        }

        private string m_Name;

        public abstract bool CheckCondition(IDialogConditionTarget target);

        public string name {
            get {
                return m_Name;
            }
        }
    }
}
