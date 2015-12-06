using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Store {
    public abstract class ItemPrice {
        public InventoryObjectType type { get; private set; }
        public int price { get; private set; }

        public ItemPrice(Hashtable hash) {
            type = String2Type((string)hash.GetValueString((int)SPC.Type));
            price = hash.GetValueInt((int)SPC.Price);
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
