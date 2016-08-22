using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Dialogs {
    public interface IDialogConditionTarget {
        bool OnStation();
        bool AtSpace();
        bool IsQuestCompleted(string id);
        bool IsDialogCompleted(string id);
    }
}
