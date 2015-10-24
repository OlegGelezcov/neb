using Common;
using ServerClientCommon;
using Space.Game.Inventory;
using System;
using System.Collections;

namespace Nebula.Inventory.Objects {
    /// <summary>
    /// Server pass inventory object (incapsulate single pass at inventory)
    /// </summary>
    public class PassInventoryObject : IInventoryObject {

        //pass fully described with his ID
        private string mID;

        //no name for compatibility
        private string mName;
        //no level ( remain for compatibility)
        private int mLevel;
        //no color (remain for compatibility)
        private ObjectColor mColor;
        //raw pass info ( for auction )
        private Hashtable mRaw;

        public bool binded { get; private set; } = false;

        public bool splittable {
            get {
                return false;
            }
        }

        public int Level {
            get {
                return mLevel;
            }
        }

        public string Id {
            get {
                return mID;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.pass;
            }
        }

        public Hashtable rawHash {
            get {
                return mRaw;
            }
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        /// <summary>
        /// Bind operation don't support 
        /// </summary>
        public void Bind() {
            binded = true;
            throw new NotSupportedException("pass not allow bind");
        }

        public Hashtable GetInfo() {
            Hashtable info = new Hashtable {
                {(int)SPC.Id, Id },
                {(int)SPC.Name, mName},
                {(int)SPC.Level, Level},
                {(int)SPC.ItemType, (int)(byte)Type},
                {(int)SPC.Color, (int)(byte)mColor},
                {(int)SPC.PlacingType, placingType },
                {(int)SPC.Binded, binded },
                {(int)SPC.Splittable, splittable}
            };
            return info;
        }

        public void ParseInfo(Hashtable info) {
            mRaw = info;
            mID = info.GetValue<string>((int)SPC.Id, string.Empty);
            mName = string.Empty;
            mLevel = 1;
            mColor = ObjectColor.white;
        }

        public PassInventoryObject(string id) {
            mID = id;
            mName = string.Empty;
            mLevel = 1;
            mColor = ObjectColor.white;
            mRaw = GetInfo();
        }

        public PassInventoryObject(Hashtable info) {
            ParseInfo(info);
        }
    }
}
