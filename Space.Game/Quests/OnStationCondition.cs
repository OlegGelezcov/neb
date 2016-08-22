using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class OnStationCondition : QuestCondition {
        public override bool CheckCondition(IQuestConditionTarget target, object userEvent = null) {
            return target.isStation();
        }
    }
}
