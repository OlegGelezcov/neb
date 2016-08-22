using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Dialogs {
    public abstract class DialogCondition {
        public abstract bool CheckCondition(IDialogConditionTarget target);
    }
}
