using Common;
using ServerClientCommon;
using Space.Game.Inventory;
using System.Collections;

namespace Nebula.Inventory.Objects {
    public class CreditsBagObject : IInventoryObject {

        private Hashtable m_Raw;
        private int m_Count;
        private bool m_IsNew;
        

        public CreditsBagObject(Hashtable hash) {
            ParseInfo(hash);
        }

        public CreditsBagObject(string id, int count, bool binded) {
            this.Id = id;
            this.m_Count = count;
            this.binded = binded;
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
            get {
                return m_IsNew;
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
                return true;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.credits_bag;
            }
        }

        public void Bind() {
            binded = true;
        }

        public void SetNew(bool val) {
            m_IsNew = val;
        }

        public void ResetNew() {
            m_IsNew = false;
        }

        public int count {
            get {
                return m_Count;
            }
        }

        public void ParseInfo(Hashtable info) {
            m_Raw = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            m_Count = info.GetValue<int>((int)SPC.Value, 0);
            m_IsNew = info.GetValue<bool>((int)SPC.IsNew, false);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
        }
        public Hashtable GetInfo() {
            var hash = InventoryUtils.ItemHash(Id, Level, ObjectColor.white, InventoryObjectType.credits_bag, PlacingType.Inventory, binded, splittable);
            hash.Add((int)SPC.Value, count);
            hash.Add((int)SPC.IsNew, m_IsNew);
            return hash;
        }
    }
}
