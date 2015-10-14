using Common;
using ServerClientCommon;
using Space.Game.Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Inventory.Objects {
    public class FortificationInventoryObject : IInventoryObject{
        public int race { get; private set; }
        private Hashtable mRaw;

        public FortificationInventoryObject(Hashtable info) {
            ParseInfo(info);
        }
        public FortificationInventoryObject(string inID, int inRace, bool inBinded = false) {
            Id = inID;
            race = inRace;
            binded = inBinded;
        }

        public bool binded {
            get;
            private set;
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
                mRaw = GetInfo();
                return mRaw;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.fortification;
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
            return new Hashtable {
                { (int)SPC.Id, Id },
                { (int)SPC.Level, Level },
                { (int)SPC.Color, (int)(byte)ObjectColor.white },
                { (int)SPC.ItemType, (int)(byte)Type },
                { (int)SPC.PlacingType, placingType },
                { (int)SPC.Binded, binded },
                { (int)SPC.Race, race },
                { (int)SPC.Splittable, splittable }
            };
        }

        public void ParseInfo(Hashtable info) {
            mRaw = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
            race = info.GetValue<int>((int)SPC.Race, (int)(byte)Race.None);
        }
    }
}
