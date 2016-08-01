using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Inventory.Objects {
    public class PlanetCommandCenterInventoryObjectInfo : IInventoryObjectInfo {

        public PlanetCommandCenterInventoryObjectInfo(Hashtable hash) {
            ParseInfo(hash);
        }

        public bool binded {
            get;
            private set;
        }

        public string Id {
            get;
            private set;
        }

        public bool isNew {
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
                return InventoryObjectType.planet_command_center;
            }
        }

        public Hashtable GetInfo() {
            return this.rawHash;
        }

        public bool HasColor() {
            return false;
        }

        public ObjectColor MyColor() {
            return ObjectColor.white;
        }

        public void ParseInfo(Hashtable info) {
            this.rawHash = info;
            this.Id = info.GetValueString((int)SPC.Id);
            this.isNew = info.GetValueBool((int)SPC.IsNew);
            this.binded = info.GetValueBool((int)SPC.Binded);
        }
    }
}
