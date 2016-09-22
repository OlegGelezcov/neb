using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests.Drop {
    public class QuestObjectDropItem : DropItem {

        public string quest { get; private set; }

        public QuestObjectDropItem(string itemId,  int count, string quest)
            : base(itemId, InventoryObjectType.quest_item, count) {
            this.quest = quest;
        }
    }
}
