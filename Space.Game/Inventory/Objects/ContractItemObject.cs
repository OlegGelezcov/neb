using Common;
using ServerClientCommon;
using Space.Game.Inventory;
using System.Collections;

namespace Nebula.Inventory.Objects {
    public class ContractItemObject : IInventoryObject {

        private string m_Id;
        private string m_ContractId;
        private Hashtable m_Raw;

        public ContractItemObject(string id, string contractId) {
            m_Id = id;
            m_ContractId = contractId;
        }

        public ContractItemObject(Hashtable hash) {
            ParseInfo(hash);
        }

        public string contractId {
            get {
                return m_ContractId;
            }
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

        public int Level {
            get {
                return 1;
            }
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable rawHash {
            get {
                return m_Raw;
            }
        }

        public bool splittable {
            get {
                return false;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.contract_item;
            }
        }

        public void Bind() {
            
        }

        public Hashtable GetInfo() {
            m_Raw = InventoryUtils.ItemHash(Id, Level, ObjectColor.white, Type, (PlacingType)placingType, binded, splittable);
            m_Raw.Add((int)SPC.Contract, contractId);
            return m_Raw;
        }

        public void ParseInfo(Hashtable info) {
            m_Raw = info;
            m_Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            m_ContractId = info.GetValue<string>((int)SPC.Contract, string.Empty);
        }
    }
}
