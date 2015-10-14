using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Inventory.Objects {
    public class FortUpgradeInventoryObjectInfo : IInventoryObjectInfo, IRaceableInventoryObject{

        public FortUpgradeInventoryObjectInfo(Hashtable info) {
            ParseInfo(info);
        }

        public int minLevel { get; private set; }
        public int maxLevel { get; private set; }
        public int race { get; private set; }

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
                return InventoryObjectType.fort_upgrade;
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
            minLevel = info.GetValue<int>((int)SPC.MinLevel, 0);
            maxLevel = info.GetValue<int>((int)SPC.MaxLevel, 0);
            race = info.GetValue<int>((int)SPC.Race, (int)(byte)Race.None);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
        }
    }
}
