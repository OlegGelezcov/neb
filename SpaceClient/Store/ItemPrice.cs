using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Store {
    public abstract class ItemPrice {
        public InventoryObjectType type { get; private set; }
        public int price { get; private set; }

        public ItemPrice(Hashtable hash ) {
            type = String2Type((string)hash.GetValue<string>((int)SPC.Type, string.Empty));
            price = hash.GetValue<int>((int)SPC.Price, 0);
        }

        private InventoryObjectType String2Type(string str) {
            switch (str.Trim().ToLower()) {
                case "module":
                    return InventoryObjectType.Module;
                case "weapon":
                    return InventoryObjectType.Weapon;
                case "scheme":
                    return InventoryObjectType.Scheme;
                case "ore":
                    return InventoryObjectType.Material;
                case "nebula_element":
                    return InventoryObjectType.nebula_element;
                default:
                    return InventoryObjectType.Weapon;
            }
        }
    }
}
