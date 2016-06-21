using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Inventory.Objects {
    public class CreditsBagObjectInfo : IInventoryObjectInfo {

        private Hashtable m_Raw;

        public CreditsBagObjectInfo(Hashtable hash) {
            ParseInfo(hash);
        }

        public bool binded {
            get;
            private set;
        }

        public string Id {
            get;
            private set;
        }

        public bool isNew {
            get;
            private set;
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public int count {
            get;
            private set;
        }

        public Hashtable rawHash {
            get {
                return m_Raw;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.credits_bag;
            }
        }

        public Hashtable GetInfo() {
            return m_Raw;
        }

        public bool HasColor() {
            return false;
        }

        public ObjectColor MyColor() {
            return ObjectColor.white;
        }

        public void ParseInfo(Hashtable info) {
            m_Raw = info;
            isNew = info.GetValueBool((int)SPC.IsNew);
            Id = info.GetValueString((int)SPC.Id);
            binded = info.GetValueBool((int)SPC.Binded);
            count = info.GetValueInt((int)SPC.Value);
        }
    }
}
