using Space.Game.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Inventory.Objects {
    public class LootBoxObject : IInventoryObject {

        private Hashtable m_Raw;
        private string m_DropList;
        private bool m_IsNew;

        public LootBoxObject(Hashtable hash) { ParseInfo(hash);  }

        public LootBoxObject(string id, string dropList) {
            Id = id;
            m_DropList = dropList;
            m_IsNew = true;
        }


        public bool binded {
            get {
                return true;
            }
        }

        public string Id {
            get;
            private set;
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

        public string dropList {
            get {
                return m_DropList;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.loot_box;
            }
        }

        public void Bind() {
            
        }
        public bool isNew {
            get {
                return m_IsNew;
            }
        }
        public void ResetNew() {
            m_IsNew = false;
        }
        public void SetNew(bool val) {
            m_IsNew = val;
        }
        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Id, Id },
                { (int)SPC.Level, Level },
                { (int)SPC.Color, (int)(byte)ObjectColor.white },
                { (int)SPC.ItemType, (int)(byte)Type },
                { (int)SPC.PlacingType, placingType },
                { (int)SPC.Binded, binded },
                { (int)SPC.Splittable, splittable },
                { (int)SPC.DropList, dropList  },
                { (int)SPC.IsNew, isNew }
            };
        }

        public void ParseInfo(Hashtable info) {
            m_Raw = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            m_DropList = info.GetValue<string>((int)SPC.DropList, string.Empty);
            m_IsNew = info.GetValue<bool>((int)SPC.IsNew, false);

        }
    }
}
