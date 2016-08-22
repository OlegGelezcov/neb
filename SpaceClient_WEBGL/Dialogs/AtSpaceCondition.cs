using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Dialogs {
    public class AtSpaceCondition : DialogCondition {

        public override bool CheckCondition(IDialogConditionTarget target) {
            return target.AtSpace();
        }
    }
}
