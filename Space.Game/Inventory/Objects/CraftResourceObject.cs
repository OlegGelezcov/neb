using Common;
using ServerClientCommon;
using Space.Game.Inventory;
using System.Collections;

namespace Nebula.Inventory.Objects {
    public class CraftResourceObject : IInventoryObject {
        private string m_Id;
        private Hashtable m_Raw;
        private ObjectColor m_Color;

        public CraftResourceObject(string id, ObjectColor color, bool inBinded = false) {
            m_Id = id;
            binded = inBinded;
            m_Color = color;
        }

        public CraftResourceObject(Hashtable hash) {
            ParseInfo(hash);
        }

        public bool binded {
            get;
            private set;
        } = false;

        public bool splittable {
            get {
                return false;
            }
        }

        public int Level {
            get {
                return 1;
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


        public void Bind() {
            binded = true;
        }

        public ObjectColor color {
            get {
                return m_Color;
            }
        }

        public Hashtable GetInfo() {
            m_Raw = new Hashtable {
                { (int)SPC.Id, Id },
                { (int)SPC.Level, Level },
                { (int)SPC.ItemType, (int)(byte)Type},
                { (int)SPC.PlacingType, placingType },
                { (int)SPC.Binded, binded },
                { (int)SPC.Splittable, splittable },
                { (int)SPC.Color, (int)(byte)color}
            };
            return m_Raw;
        }

        public void ParseInfo(Hashtable info) {
            m_Raw = info;
            m_Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
            m_Color = (ObjectColor)(byte)info.GetValue<int>((int)SPC.Color, (int)(byte)ObjectColor.white);
        }
    }
}
