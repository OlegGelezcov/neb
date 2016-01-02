using MongoDB.Bson;
using Space.Game;
using System.Collections;

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
