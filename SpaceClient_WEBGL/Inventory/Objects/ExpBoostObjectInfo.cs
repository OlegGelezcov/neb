using Common;
using ExitGames.Client.Photon;
using ServerClientCommon;


namespace Nebula.Client.Inventory.Objects {
    public class ExpBoostObjectInfo : IInventoryObjectInfo{

        public ExpBoostObjectInfo(Hashtable hash) { ParseInfo(hash); }

        public int tag {
            get;
            private set;
        }

        public int interval {
            get;
            private set;
        }

        public float value {
            get;
            private set;
        }

        public int level {
            get;
            private set;
        }

        #region Interface IInventoryObjectInfo
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
                return InventoryObjectType.exp_boost;
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
            value = info.GetValue<float>((int)SPC.Value, 0f);
            interval = info.GetValue<int>((int)SPC.Interval, interval);
            tag = info.GetValue<int>((int)SPC.Tag, 0);
        } 
        #endregion
    }
}
