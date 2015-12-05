using Common;
using Nebula.Client.Inventory.Objects;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class ClientPlayerShipWeapon : IInfoParser {
        private bool hasWeapon;
        public float damage { get; private set; }
        public float critDamage { get; private set; }

        private WeaponInventoryObjectInfo weaponObject;

        public float optimalDistance { get; private set; }

        public ClientPlayerShipWeapon() { }

        public ClientPlayerShipWeapon(Hashtable info) {
            this.ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            this.hasWeapon = info.GetValueBool((int)SPC.HasWeapon);
            this.optimalDistance = info.GetValueFloat((int)SPC.OptimalDistance);
            this.damage = info.GetValueFloat((int)SPC.Damage);
            this.critDamage = info.GetValueFloat((int)SPC.CritDamage);

            if (this.hasWeapon) {
                if (this.weaponObject == null)
                    this.weaponObject = new WeaponInventoryObjectInfo();

                this.weaponObject.ParseInfo(info.GetValueHash((int)SPC.Source));
            } else {
                this.weaponObject = null;
            }
        }

        public bool HasWeapon {
            get {
                return this.hasWeapon;
            }
        }


        public WeaponInventoryObjectInfo WeaponObject {
            get {
                return this.weaponObject;
            }
        }
    }
}
