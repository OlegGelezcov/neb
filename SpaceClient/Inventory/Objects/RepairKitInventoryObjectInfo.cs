using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Inventory.Objects {
    public class RepairKitInventoryObjectInfo : IInventoryObjectInfo {
        public RepairKitInventoryObjectInfo(Hashtable info) {
            ParseInfo(info);
        }

        public float value {
            get;
            private set;
        }

        public bool binded {
            get;
            private set;
        }

        public string Id {
            get;
            private set;
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable rawHash {
            get;
            private set;
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.repair_kit;
            }
        }

        public Hashtable GetInfo() {
            return rawHash;
        }

        public bool HasColor() {
            return false;
        }

        public ObjectColor MyColor() {
            return ObjectColor.white;
        }

        public void ParseInfo(Hashtable info) {
            rawHash = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
            value = info.GetValue<float>((int)SPC.Value, 0f);
        }
    }
}
