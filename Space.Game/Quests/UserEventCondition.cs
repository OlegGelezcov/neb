using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class UserEventCondition : QuestCondition {
        public override bool CheckCondition(IQuestConditionTarget target, object data = null) {
            if(data != null ) {
                if(data is UserEvent  ) {
                    return CheckEventCondition(target, data as UserEvent);
                }
            }
            return false;
        }

        protected virtual bool CheckEventCondition(IQuestConditionTarget target, UserEvent userEvent ) {
            return false;
        }
    }

    public class UserEventNameCondition : UserEventCondition {
        private UserEventName m_Name;

        private UserEventName name {
            get {
                return m_Name;
            }
        }

        public UserEventNameCondition(UserEventName nm) {
            m_Name = nm;
        }

        protected override bool CheckEventCondition(IQuestConditionTarget target, UserEvent userEvent) {
            return (userEvent.name == name);
        }
    }

    //public class RotateCameraUserEventCondition : UserEventCondition {

    //    protected override bool CheckEventCondition(IQuestConditionTarget target, UserEvent userEvent) {
    //        if(userEvent.name == Common.UserEventName.rotate_camera ) {
    //            return true;
    //        }
    //        return false;
    //    }
    //}

    //public class StartMovingUserEventCondition : UserEventCondition {
    //    protected override bool CheckEventCondition(IQuestConditionTarget target, UserEvent userEvent) {
    //        if(userEvent.name == Common.UserEventName.start_moving) {
    //            return true;
    //        }
    //        return false;
    //    }
    //}
}
