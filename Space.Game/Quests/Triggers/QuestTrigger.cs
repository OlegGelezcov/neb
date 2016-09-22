using Common;
using Nebula.Game.Events;
using Nebula.Quests.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests.Triggers {
    public abstract class QuestTrigger {
        public QuestTriggerType type { get; private set; }
        public EventType eventType { get; private set; }
        public List<PostAction> postActions { get; private set; }

        public QuestTrigger(QuestTriggerType type, EventType eventType, List<PostAction> postActions) {
            this.type = type;
            this.eventType = eventType;
            this.postActions = postActions;
        }

        public bool Execute(IQuestConditionTarget target, BaseEvent evt) {
            if(Check(target, evt)) {
                if(postActions != null ) {
                    target.ExecutePostActionList(postActions);
                }
                return true;
            }
            return false;
        }

        protected abstract bool Check(IQuestConditionTarget target, BaseEvent evt);

    }
}
