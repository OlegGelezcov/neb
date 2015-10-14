using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using ServerClientCommon;

namespace Nebula.Client.Inventory.Objects {
    public class MiningStationInventoryObjectInfo : IInventoryObjectInfo, IRaceableInventoryObject {

        public MiningStationInventoryObjectInfo(Hashtable info) {
            ParseInfo(info);
        }

        public int speed { get; private set; }
        public int capacity { get; private set; }
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
                return InventoryObjectType.mining_station;
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
            speed = info.GetValue<int>((int)SPC.Speed, 0);
            capacity = info.GetValue<int>((int)SPC.Capacity, 0);
        }
    }
}
