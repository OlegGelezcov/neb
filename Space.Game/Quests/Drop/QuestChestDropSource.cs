using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests.Drop {
    public class QuestChestDropSource : DropSource {

        public string quest { get; private set; }

        public QuestChestDropSource(string quest) : base(Common.ItemType.QuestChest) {
            this.quest = quest;
        }

    }
}
