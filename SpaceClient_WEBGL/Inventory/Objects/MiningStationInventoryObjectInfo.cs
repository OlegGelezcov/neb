using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Inventory.Objects {
    public class MiningStationInventoryObjectInfo : IInventoryObjectInfo, IRaceableInventoryObject {

        public MiningStationInventoryObjectInfo(Hashtable info) {
            ParseInfo(info);
        }

        public int speed { get; private set; }
        public int capacity { get; private set; }
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
                return InventoryObjectType.mining_station;
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
            binded = info.GetValueBool((int)SPC.Binded);
            race = info.GetValueInt((int)SPC.Race, (int)(byte)Race.None);
            speed = info.GetValueInt((int)SPC.Speed);
            capacity = info.GetValueInt((int)SPC.Capacity);
        }
    }
}
