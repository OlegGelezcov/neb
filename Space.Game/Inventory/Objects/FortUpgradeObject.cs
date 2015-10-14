﻿using Space.Game.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Inventory.Objects {
    public class FortUpgradeObject : IInventoryObject {

        public int minLevel { get; private set; }
        public int maxLevel { get; private set; }
        public int race { get; private set; }

        private Hashtable mRaw;
        public FortUpgradeObject(Hashtable info) {
            ParseInfo(info);
        }
        public FortUpgradeObject(string inID, int inMinLevel, int inMaxLevel, bool inBinded = false) {
            Id = inID;
            minLevel = inMinLevel;
            maxLevel = inMaxLevel;
            binded = inBinded;
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
                return InventoryObjectType.fort_upgrade;
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
                { (int)SPC.Splittable, splittable}
            };
        }

        public void ParseInfo(Hashtable info) {
            mRaw = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            minLevel = info.GetValue<int>((int)SPC.MinLevel, 0);
            maxLevel = info.GetValue<int>((int)SPC.MaxLevel, 0);
            race = info.GetValue<int>((int)SPC.Race, (int)(byte)Race.None);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
        }
    }
}
