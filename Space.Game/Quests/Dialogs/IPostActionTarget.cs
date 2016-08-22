using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests.Dialogs {
    public interface IPostActionTarget {
        bool StartQuest(string id);
    }
}
