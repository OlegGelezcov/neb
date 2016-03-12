using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Inventory.Objects {
    public class ContractItemInventoryObjectInfo : IInventoryObjectInfo {

        private string m_Id;
        private string m_ContractId;

        public ContractItemInventoryObjectInfo(Hashtable hash) {
            ParseInfo(hash);
        }

        public bool isNew {
            get;
            private set;
        }

        public bool binded {
            get {
                return true;
            }
        }

        public string Id {
            get {
                return m_Id;
            }
        }

        public string contractId {
            get {
                return m_ContractId;
            }
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
                return InventoryObjectType.contract_item;
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
            m_Id = info.GetValueString((int)SPC.Id);
            m_ContractId = info.GetValueString((int)SPC.Contract);
            isNew = info.GetValueBool((int)SPC.IsNew);
        }
    }
}
