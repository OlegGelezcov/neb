using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

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
        public bool isNew {
            get;
            private set;
        }

        public void ParseInfo(Hashtable info) {
            rawHash = info;
            Id = info.GetValueString((int)SPC.Id);
            binded = info.GetValueBool((int)SPC.Binded);
            value = info.GetValueFloat((int)SPC.Value);
            isNew = info.GetValueBool((int)SPC.IsNew);
        }
    }
}
