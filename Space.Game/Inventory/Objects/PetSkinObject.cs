using Common;
using ServerClientCommon;
using Space.Game.Inventory;
using System.Collections;

namespace Nebula.Inventory.Objects {
    public class PetSkinObject : IInventoryObject {

        private string m_Id;
        private bool m_Binded;
        private string m_Skin;
        private Hashtable m_Raw;

        public PetSkinObject(string id, string skin, bool binded = false) {
            m_Id = id;
            m_Binded = binded;
            m_Skin = skin;
            isNew = true;
            var dump = GetInfo();
        }

        public PetSkinObject(Hashtable hash) {
            ParseInfo(hash);
        }


        #region IInventoryObject
        public bool isNew {
            get;
            private set;
        }
        public void ResetNew() {
            isNew = false;
        }
        public void SetNew(bool val) {
            isNew = val;
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

        public int Level {
            get {
                return 0;
            }
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable rawHash {
            get {
                if (m_Raw == null) {
                    m_Raw = GetInfo();
                }
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
                return InventoryObjectType.pet_skin;
            }
        }

        public void Bind() {
            m_Binded = true;
        }

        public Hashtable GetInfo() {
            Hashtable hash = InventoryUtils.ItemHash(Id, Level, ObjectColor.white, Type, (PlacingType)placingType, binded, splittable);
            hash.Add((int)SPC.Skin, skin);
            hash.Add((int)SPC.IsNew, isNew);
            m_Raw = hash;
            return hash;
        }

        public void ParseInfo(Hashtable info) {
            m_Raw = info;
            m_Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            m_Binded = info.GetValue<bool>((int)SPC.Binded, false);
            m_Skin = info.GetValue<string>((int)SPC.Skin, string.Empty);
            isNew = info.GetValue<bool>((int)SPC.IsNew, false);
        }

        #endregion

        public string skin {
            get {
                return m_Skin;
            }
        }
    }
}
