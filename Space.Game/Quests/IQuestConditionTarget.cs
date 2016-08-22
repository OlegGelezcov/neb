using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public interface IQuestConditionTarget {
        bool isSpace();
        bool isStation();
        bool isQuestCompleted(string id);
        bool isDialogCompleted(string id);
    }
}
