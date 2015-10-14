using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Space.Game;
using Space.Game.Inventory.Objects;
using Nebula.Game.Components;

namespace Space.Database {
    public class WeaponDocument {
        public ObjectId Id { get; set; }

        public string CharacterId { get; set; }

        public Hashtable WeaponObject { get; set; }

        public bool IsNewDocument { get; set; }



        public void Set(ShipWeaponSave sourceObject) {
            WeaponObject = sourceObject.weaponObject;
            this.IsNewDocument = false;
        }

        public ShipWeaponSave SourceObject(IRes resource) {
            return new ShipWeaponSave {
                characterID = CharacterId,
                weaponObject = WeaponObject
            };
        }
    }

    public class ShipWeaponSave {
        public string characterID;
        public Hashtable weaponObject;
    }


}
