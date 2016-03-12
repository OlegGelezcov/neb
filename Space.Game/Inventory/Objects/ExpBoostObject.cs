using Common;
using ServerClientCommon;
using Space.Game.Inventory;
using System;
using System.Collections;

namespace Nebula.Inventory.Objects {
    public class ExpBoostObject : IInventoryObject {

        private Hashtable m_Raw;

        private float m_Value;
        private int m_Interval;
        private int m_Tag;
        private bool m_IsNew;

        public ExpBoostObject(Hashtable hash) { ParseInfo(hash); }

        public ExpBoostObject(string id, float value, int interval, int tag) {
            Id = id;
            m_Value = value;
            m_Interval = interval;
            m_Tag = tag;
            m_IsNew = true;

            m_Raw = GetInfo();

        }

        #region Interface IInventoryObject
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

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.exp_boost;
            }
        }

        public void Bind() { }

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
                { (int)SPC.Value, value },
                { (int)SPC.Interval, interval },
                { (int)SPC.Tag, tag },
                { (int)SPC.IsNew, isNew }
            };
        }

        public void ParseInfo(Hashtable info) {
            m_Raw = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            m_Value = info.GetValue<float>((int)SPC.Value, 0f);
            m_Interval = info.GetValue<int>((int)SPC.Interval, 0);
            m_Tag = info.GetValue<int>((int)SPC.Tag, 0);
            m_IsNew = info.GetValue<bool>((int)SPC.IsNew, false);
        } 
        #endregion

        public float value {
            get {
                return m_Value;
            }
        }
        public int interval {
            get {
                return m_Interval;
            }
        }
        public int tag {
            get {
                return m_Tag;
            }
        }
    }
}
