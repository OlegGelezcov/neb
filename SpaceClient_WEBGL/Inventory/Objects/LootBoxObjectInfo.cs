using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Inventory.Objects {
    public class LootBoxObjectInfo : IInventoryObjectInfo {

        public LootBoxObjectInfo(Hashtable hash) {
            ParseInfo(hash);
        }

        public bool isNew {
            get;
            private set;
        }

        public string dropList {
            get;
            private set;
        }

        public int level {
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
                return InventoryObjectType.loot_box;
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
            level = info.GetValue<int>((int)SPC.Level, 0);
            binded = info.GetValue<bool>((int)SPC.Binded, true);
            dropList = info.GetValue<string>((int)SPC.DropList, string.Empty);
            isNew = info.GetValueBool((int)SPC.IsNew);
        }
    }
}
