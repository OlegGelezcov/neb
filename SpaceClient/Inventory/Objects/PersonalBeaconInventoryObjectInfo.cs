using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using ServerClientCommon;

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
            interval = info.GetValue<int>((int)SPC.Interval, 0);
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
        } 
        #endregion
    }
}
