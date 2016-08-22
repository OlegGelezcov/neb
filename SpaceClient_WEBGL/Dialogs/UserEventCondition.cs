using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Dialogs {
    public class UserEventCondition : DialogCondition {

        private UserEventName m_Name;

        public UserEventName name {
            get {
                return m_Name;
            }
        }

        public UserEventCondition(UserEventName nm) {
            m_Name = nm;
        }

        public override bool CheckCondition(IDialogConditionTarget target) {


            //always false on client
            return false;
        }
    }
}
