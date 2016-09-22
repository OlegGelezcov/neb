using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class OnStationCondition : QuestCondition {

        public OnStationCondition() 
            : base(QuestConditionName.ON_STATION) { }

        public override bool CheckCondition(IQuestConditionTarget target, object userEvent = null) {
            return target.isStation();
        }
    }
}
