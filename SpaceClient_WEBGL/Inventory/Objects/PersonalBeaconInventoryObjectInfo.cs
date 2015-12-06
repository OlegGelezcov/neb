﻿using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Inventory.Objects {
    public class PersonalBeaconInventoryObjectInfo : IInventoryObjectInfo {

        public int interval { get; private set; }

        public PersonalBeaconInventoryObjectInfo(Hashtable info) {
            ParseInfo(info);
        }

        #region IInventoryObjectInfo interface
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
                return InventoryObjectType.personal_beacon;
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
            interval = info.GetValueInt((int)SPC.Interval);
            Id = info.GetValueString((int)SPC.Id);
            binded = info.GetValueBool((int)SPC.Binded);
        }
        #endregion
    }
}
