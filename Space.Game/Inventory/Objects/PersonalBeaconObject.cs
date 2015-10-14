using Common;
using ServerClientCommon;
using Space.Game.Inventory;
using System.Collections;
using System;

namespace Nebula.Inventory.Objects {
    public class PersonalBeaconObject : IInventoryObject {

        private Hashtable mRaw;

        public bool binded {
            get;
            private set;
        }

        public string Id {
            get;
            private set;
        }

        public PersonalBeaconObject(Hashtable info) {
            ParseInfo(info);
        }

        public PersonalBeaconObject(string inID, int inInterval, bool inBinded = false) {
            Id = inID;
            interval = inInterval;
            binded = inBinded;
        }

        public int Level {
            get { return 1; }
        }

        public int interval { get; private set; }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable rawHash {
            get {
                mRaw = GetInfo();
                return mRaw;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.personal_beacon;
            }
        }

        public bool splittable {
            get {
                return true;
            }
        }

        public void Bind() {
            binded = true;
        }

        public Hashtable GetInfo() {
            mRaw = new Hashtable {
                    { (int)SPC.Id, Id },
                    { (int)SPC.Level, Level },
                    { (int)SPC.Color, (int)(byte)ObjectColor.white },
                    { (int)SPC.ItemType, (int)(byte)Type },
                    { (int)SPC.PlacingType, placingType },
                    { (int)SPC.Binded, binded },
                    { (int)SPC.Interval, interval },
                    { (int)SPC.Splittable, splittable }
                };
            return mRaw;
        }

        public void ParseInfo(Hashtable info) {
            mRaw = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            interval = info.GetValue<int>((int)SPC.Interval, 0);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
        }
    }
}
