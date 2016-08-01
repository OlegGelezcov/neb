using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Inventory.Objects {
    public class PlanetTurretInventoryObjectInfo : IInventoryObjectInfo {

        public PlanetTurretInventoryObjectInfo(Hashtable hash) {
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
                return InventoryObjectType.planet_turret;
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
