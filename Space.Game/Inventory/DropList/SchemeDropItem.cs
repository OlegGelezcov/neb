using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Inventory.DropList {
    public class SchemeDropItem : DropItem {
        public SchemeDropItem(int min, int max, float prob)
            : base(min, max, prob, Common.InventoryObjectType.Scheme) { }
    }
}
