using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Inventory.Objects {
    public class PetSchemeInventoryObjectInfo : IInventoryObjectInfo {

        private string m_Id;
        private bool m_Binded;
        private PetColor m_PetColor;
        private Hashtable m_Raw;

        public PetSchemeInventoryObjectInfo(Hashtable hash) {
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
                return InventoryObjectType.pet_scheme;
            }
        }

        public Hashtable GetInfo() {
            return rawHash;
        }

        public bool HasColor() {
            return true;
        }

        public ObjectColor MyColor() {
            return EnumUtils.Pet2ObjColor(m_PetColor);
        }
        public bool isNew {
            get;
            private set;
        }

        public void ParseInfo(Hashtable info) {
            m_Raw = info;
            m_Id = info.GetValueString((int)SPC.Id);
            m_Binded = info.GetValueBool((int)SPC.Binded);
            m_PetColor = (PetColor)info.GetValueInt((int)SPC.PetColor, (int)PetColor.gray);
            isNew = info.GetValueBool((int)SPC.IsNew);
        } 
        #endregion

        public PetColor petColor {
            get {
                return m_PetColor;
            }
        }
    }
}
