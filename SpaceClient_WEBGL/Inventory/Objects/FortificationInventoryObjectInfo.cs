﻿using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Inventory.Objects {
    public class FortificationInventoryObjectInfo : IInventoryObjectInfo, IRaceableInventoryObject {

        public FortificationInventoryObjectInfo(Hashtable info) {
            ParseInfo(info);
        }

        public bool isNew {
            get;
            private set;
        }

        public int race { get; private set; }


        public Race Race {
            get {
                return (Race)(byte)race;
            }
        }

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
                return InventoryObjectType.fortification;
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
            Id = info.GetValueString((int)SPC.Id);
            binded = info.GetValueBool((int)SPC.Binded);
            race = info.GetValueInt((int)SPC.Race, (int)(byte)Race.None);
            isNew = info.GetValueBool((int)SPC.IsNew);
        }
    }
}
