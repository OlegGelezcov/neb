using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests.Drop {
    public class DropItem {
        public string itemId { get; private set; }
        public InventoryObjectType itemType { get; private set; }
        public int count { get; private set; }
        
        public DropItem(string itemId, InventoryObjectType itemType, int count ) {
            this.itemId = itemId;
            this.itemType = itemType;
            this.count = count;
        } 
    }
}
