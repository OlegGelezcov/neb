using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Dialogs {
    public class OnStationCondition : DialogCondition {

        public OnStationCondition() : base(QuestConditionName.ON_STATION) { }

        public override bool CheckCondition(IDialogConditionTarget target) {
            return target.OnStation();
        }
    }
}
