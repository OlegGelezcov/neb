﻿using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Inventory.Objects {
    public class TurretInventoryObjectInfo : IInventoryObjectInfo, IRaceableInventoryObject {
        public TurretInventoryObjectInfo(Hashtable info) {
            ParseInfo(info);
        }

        public int race { get; private set; }
        public bool binded {
            get;
            private set;
        }

        public string Id {
            get;
            private set;
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable rawHash {
            get;
            private set;
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.turret;
            }
        }

        public Hashtable GetInfo() {
            return rawHash;
        }

        public bool HasColor() {
            return false;
        }

        public ObjectColor MyColor() {
            return ObjectColor.white;
        }

        public void ParseInfo(Hashtable info) {
            rawHash = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
            race = info.GetValue<int>((int)SPC.Race, (int)(byte)Race.None);
        }
    }
}