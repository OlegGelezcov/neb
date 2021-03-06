﻿using Common;
using ServerClientCommon;
using Space.Game.Inventory;
using System.Collections;
using System;

namespace Nebula.Inventory.Objects {
    public class OutpostUpgradeObject : IInventoryObject {

        public int minLevel { get; private set; }
        public int maxLevel { get; private set; }
        public int race { get; private set; }

        private Hashtable mRaw;
        public OutpostUpgradeObject(Hashtable info) {
            ParseInfo(info);
        }
        public OutpostUpgradeObject(int inMinLevel, int inMaxLevel, Race inrace, bool inBinded = false) {
            Id = "outpost_upgrade_object_" + inMinLevel.ToString() + inMaxLevel.ToString() + ((int)inrace).ToString();
            minLevel = inMinLevel;
            maxLevel = inMaxLevel;
            binded = inBinded;
            race = (int)(byte)inrace;
            isNew = true;
        }

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
            get; private set;
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
                return InventoryObjectType.out_upgrade;
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
                { (int)SPC.MinLevel, minLevel },
                { (int)SPC.MaxLevel, maxLevel },
                { (int)SPC.Race, race },
                { (int)SPC.Splittable, splittable},
                { (int)SPC.IsNew, isNew }
            };
        }

        public void ParseInfo(Hashtable info) {
            mRaw = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            minLevel = info.GetValue<int>((int)SPC.MinLevel, 0);
            maxLevel = info.GetValue<int>((int)SPC.MaxLevel, 0);
            race = info.GetValue<int>((int)SPC.Race, (int)(byte)Race.None);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
            isNew = info.GetValue<bool>((int)SPC.IsNew, false);
        }
    }
}
