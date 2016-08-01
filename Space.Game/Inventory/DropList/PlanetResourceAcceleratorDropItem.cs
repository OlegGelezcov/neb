using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Inventory.DropList {
    public class PlanetResourceAcceleratorDropItem : DropItem {
        public PlanetResourceAcceleratorDropItem(int min, int max, float prob)
            : base(min, max, prob, Common.InventoryObjectType.planet_resource_accelerator) { }
    }
}
