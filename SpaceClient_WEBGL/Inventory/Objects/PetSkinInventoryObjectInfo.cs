using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Inventory.Objects {
    public class PetSkinInventoryObjectInfo : IInventoryObjectInfo {

        private string m_Id;
        private bool m_Binded;
        private string m_Skin;
        private Hashtable m_Raw;

        public PetSkinInventoryObjectInfo(Hashtable hash) {
            ParseInfo(hash);
        }

        #region IInventoryObjectInfo
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

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.pet_skin;
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
            m_Raw = info;
            m_Id = info.GetValueString((int)SPC.Id);
            m_Binded = info.GetValueBool((int)SPC.Binded);
            m_Skin = info.GetValueString((int)SPC.Skin);
        } 
        #endregion

        public string skin {
            get {
                return m_Skin;
            }
        }
    }
}
