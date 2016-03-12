using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Inventory.Objects {
    public class CraftResourceInventoryObjectInfo : IInventoryObjectInfo{
        private string m_Id;
        private bool m_Binded;
        private Hashtable m_Raw;
        private ObjectColor m_Color;

        public CraftResourceInventoryObjectInfo(Hashtable hash) {
            m_Raw = hash;
            ParseInfo(hash);
        }

        public bool isNew {
            get;
            private set;
        }

        public bool binded {
            get {
                return m_Binded;
            }
        }

        public string Id {
            get {
                return m_Id;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.craft_resource;
            }
        }

        public Hashtable rawHash {
            get {
                return m_Raw;
            }
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable GetInfo() {
            return rawHash;
        }

        public void ParseInfo(Hashtable info) {
            m_Raw = info;
            m_Id = info.GetValueString((int)SPC.Id);
            m_Binded = info.GetValueBool((int)SPC.Binded);
            m_Color = (ObjectColor)(byte)info.GetValueInt((int)SPC.Color);
            isNew = info.GetValueBool((int)SPC.IsNew);
        }

        public bool HasColor() {
            return true;
        }

        public ObjectColor MyColor() {
            return m_Color;
        }
    }
}
