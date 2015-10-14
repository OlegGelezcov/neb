using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class SchemeCraftingMaterial {
        public int level { get; private set; }
        public ShipModelSlotType slotType { get; private set; }
        public ConcurrentDictionary<string, int> craftingMaterials { get; private set; }

        public SchemeCraftingMaterial(XElement element) {
            level = element.GetInt("level");
            slotType = (ShipModelSlotType)Enum.Parse(typeof(ShipModelSlotType), element.GetString("type"));
            craftingMaterials = new ConcurrentDictionary<string, int>();

            var dump = element.Elements("material").Select(m => {
                string id = m.GetString("id");
                int count = m.GetInt("count");
                craftingMaterials.TryAdd(id, count);
                return id;
            }).ToList();
        }
    }
}
