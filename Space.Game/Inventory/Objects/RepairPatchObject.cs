﻿using Common;
using ServerClientCommon;
using Space.Game.Inventory;
using System.Collections;
using System;

namespace Nebula.Inventory.Objects {
    public class RepairPatchObject : IInventoryObject {
        private Hashtable mRaw;

        public float value { get; private set; }
        public RepairPatchObject(Hashtable info) {
            ParseInfo(info);
        }
        public RepairPatchObject(string inID, float inValue, bool inBinded = false) {
            Id = inID;
            value = inValue;
            binded = inBinded;
        }

        #region IInventoryObject interface
        public bool binded {
            get;
            private set;
        }

        public string Id {
            get;
            private set;
        }

        public int Level {
            get { return 1; }
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
                return InventoryObjectType.repair_patch;
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
                    { (int)SPC.Value, value },
                { (int)SPC.Splittable, splittable }
                };
            return mRaw;
        }

        public void ParseInfo(Hashtable info) {
            mRaw = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            value = info.GetValue<float>((int)SPC.Value, 0f);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
        }
        #endregion
    }
}
