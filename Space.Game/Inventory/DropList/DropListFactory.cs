using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Inventory.DropList {
    public class DropListFactory {

        public List<DropItem> Create(XElement element) {
            List<DropItem> items = new List<DropItem>();
            DropItemFactory itemFactory = new DropItemFactory();

            var dump = element.Elements("item").Select(itemElement => {
                var it = itemFactory.Create(itemElement);
                if (it != null) {
                    items.Add(it);
                }
                return it;
            }).ToList();
            return items;
        }
    }
}
