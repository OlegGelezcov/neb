using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Inventory.Objects {
    public class FortUpgradeInventoryObjectInfo : IInventoryObjectInfo, IRaceableInventoryObject {

        public FortUpgradeInventoryObjectInfo(Hashtable info) {
            ParseInfo(info);
        }

        public int minLevel { get; private set; }
        public int maxLevel { get; private set; }
        public int race { get; private set; }

        public bool isNew {
            get;
            private set;
        }

        public bool binded {
            get;
            private set;
        }

        public Race Race {
            get {
                return (Race)(byte)race;
            }
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
            Id = info.GetValueString((int)SPC.Id);
            minLevel = info.GetValueInt((int)SPC.MinLevel);
            maxLevel = info.GetValueInt((int)SPC.MaxLevel);
            race = info.GetValueInt((int)SPC.Race, (int)(byte)Race.None);
            binded = info.GetValueBool((int)SPC.Binded);
            isNew = info.GetValueBool((int)SPC.IsNew);
        }
    }
}
