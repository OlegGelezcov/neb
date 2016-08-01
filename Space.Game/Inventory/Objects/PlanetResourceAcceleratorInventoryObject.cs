using Space.Game.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Inventory.Objects {
    public class PlanetResourceAcceleratorInventoryObject : IInventoryObject {

        public PlanetResourceAcceleratorInventoryObject() {
            this.binded = false;
            this.isNew = true;
        }

        public PlanetResourceAcceleratorInventoryObject(Hashtable hash) {
            ParseInfo(hash);
        }
        public bool binded {
            get;
            private set;
        }

        public string Id {
            get {
                return "planet_resource_accelerator";
            }
        }

        public bool isNew {
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
            get;
            private set;
        }

        public bool splittable {
            get {
                return false;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.planet_resource_accelerator;
            }
        }

        public void Bind() {
            this.binded = true;
        }

        public Hashtable GetInfo() {
            this.rawHash = InventoryUtils.ItemHash(Id, Level, ObjectColor.white, Type, (PlacingType)placingType, binded, splittable);
            return this.rawHash;
        }

        public void ParseInfo(Hashtable info) {
            this.rawHash = info;
            this.isNew = info.GetValue<bool>((int)SPC.IsNew, false);
            this.binded = info.GetValue<bool>((int)SPC.Binded, false);
        }

        public void ResetNew() {
            this.isNew = false;
        }

        public void SetNew(bool val) {
            this.isNew = val;
        }
    }
}
