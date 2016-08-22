using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class EmptyQuestCondition : QuestCondition {
        public override bool CheckCondition(IQuestConditionTarget target, object data = null) {
            return true;
        }
    }
}
