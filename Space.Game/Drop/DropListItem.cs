using Common;
using Space.Game;
using Space.Game.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Drop {
    public abstract class DropListItem {
        public InventoryObjectType category { get; private set; }
        public string colorList { get; private set; }

        public DropListItem(InventoryObjectType category, string colorList) {
            this.category = category;
            this.colorList = colorList;
        }

        public DropListItem(XElement element) {
            category = (InventoryObjectType)Enum.Parse(typeof(InventoryObjectType), element.GetString("category"));
            colorList = element.GetString("color_list");
        }

        public abstract ServerInventoryItem Roll(IRes resource, int level, Workshop workshop);
    }
}
