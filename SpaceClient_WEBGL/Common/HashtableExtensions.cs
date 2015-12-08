using ExitGames.Client.Photon;
using ServerClientCommon;

namespace Common {
    public static class HashtableExtensions {

        public static bool isInventoryObject(this Hashtable hash) {
            if(hash.ContainsKey((int)SPC.PlacingType)) {
                PlacingType placingType = (PlacingType)(int)hash[(int)SPC.PlacingType];
                if(placingType == PlacingType.Inventory) {
                    return true;
                }
            }
            return false;
        }

        public static bool isStationObject(this Hashtable hash) {
            if (hash.ContainsKey((int)SPC.PlacingType)) {
                PlacingType placingType = (PlacingType)(int)hash[(int)SPC.PlacingType];
                if (placingType == PlacingType.Station) {
                    return true;
                }
            }
            return false;
        }

        public static InventoryObjectType selectInventoryType(this Hashtable hash) {
            if( hash.ContainsKey((int)SPC.ItemType)) {
                object typeObj = hash[(int)SPC.ItemType];
                if(typeObj.GetType() == typeof(int)) {
                    return (InventoryObjectType)(byte)(int)typeObj;
                } else if(typeObj.GetType() == typeof(byte)) {
                    return (InventoryObjectType)(byte)typeObj;
                }
            }
            return InventoryObjectType.None;
        }

        public static string selectName(this Hashtable hash) {
            if(hash.ContainsKey((int)SPC.Name)) {
                return hash[(int)SPC.Name] as string;
            }
            return string.Empty;
        }

        public static ObjectColor selectColor(this Hashtable hash) {
            if(hash.ContainsKey((int)SPC.Color)) {
                object colorObject = hash[(int)SPC.Color];
                if(colorObject.GetType() == typeof(int)) {
                    return (ObjectColor)(byte)(int)colorObject;
                } else if(colorObject.GetType() == typeof(byte)) {
                    return (ObjectColor)(byte)colorObject;
                }
            }
            return ObjectColor.white;
        }

        public static Workshop selectWorkshop(this Hashtable hash) {
            if(hash.ContainsKey((int)SPC.Workshop)) {
                object workshopObj = hash[(int)SPC.Workshop];
                if(workshopObj.GetType() == typeof(int)) {
                    return (Workshop)(byte)(int)workshopObj;
                } else if(workshopObj.GetType() == typeof(byte)) {
                    return (Workshop)(byte)workshopObj;
                }
            }
            return Workshop.Arlen;
        }

        public static int selectLevel(this Hashtable hash) {
            if(hash.ContainsKey((int)SPC.Level)) {
                return (int)hash[(int)SPC.Level];
            }
            return 0;
        }

        public static string selectID(this Hashtable hash) {
            if(hash.ContainsKey((int)SPC.Id)) {
                return hash[(int)SPC.Id] as string;
            }
            return string.Empty;
        }
    }
}
