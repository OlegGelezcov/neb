using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.PvpStore {
    public class PvpStoreItem {
        public string type { get; private set; }
        public int price { get; private set; }

        public PvpStoreItem(string type, Hashtable info ) {
            this.type = type;
            price = info.GetValue<int>((int)SPC.Price, 0);
        }

        public bool isWeapon {
            get {
                return type.ToLower() == "wp";
            }
        }

        public bool isModule {
            get {
                string ltype = type.ToLower();
                return (ltype == "cb") || (ltype == "cm") || (ltype == "df") || (ltype == "dm") || (ltype == "es");
            }
        }

        public ShipModelSlotType slotType {
            get {
                switch(type.ToLower()) {
                    case "cb":
                        return ShipModelSlotType.CB;
                    case "cm":
                        return ShipModelSlotType.CM;
                    case "df":
                        return ShipModelSlotType.DF;
                    case "dm":
                        return ShipModelSlotType.DM;
                    case "es":
                        return ShipModelSlotType.ES;
                    default:
                        return ShipModelSlotType.CB;
                }
            }
        }
    }
}
