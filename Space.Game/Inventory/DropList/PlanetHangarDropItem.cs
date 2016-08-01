using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Inventory.DropList {
    public class PlanetHangarDropItem : DropItem {
        public PlanetHangarDropItem(int min, int max, float prob)
            : base(min, max, prob, InventoryObjectType.planet_resource_hangar) { }
    }
}
