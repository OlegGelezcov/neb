using Common;
using Nebula.Client.Inventory.Objects;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client {
    public class ClientPlayerShipWeapon : IInfoParser
    {
        private bool hasWeapon;
        public float damage { get; private set; }
        public float critDamage { get; private set; }

        private WeaponInventoryObjectInfo weaponObject;

        public float optimalDistance { get; private set; }

        public ClientPlayerShipWeapon() { }

        public ClientPlayerShipWeapon(Hashtable info)
        {
            this.ParseInfo(info);
        }

        public void ParseInfo(Hashtable info)
        {
            this.hasWeapon = info.GetValue<bool>((int)SPC.HasWeapon, false);
            this.optimalDistance = info.GetValue<float>((int)SPC.OptimalDistance, 0.0f);
            this.damage = info.Value<float>((int)SPC.Damage);
            this.critDamage = info.Value<float>((int)SPC.CritDamage);

            if (this.hasWeapon)
            {
                if(this.weaponObject == null)
                    this.weaponObject = new WeaponInventoryObjectInfo();

                this.weaponObject.ParseInfo(info.GetValue<Hashtable>((int)SPC.Source, new Hashtable()));
            }
            else
            {
                this.weaponObject = null;
            }
        }

        public bool HasWeapon
        {
            get
            {
                return this.hasWeapon;
            }
        }


        public WeaponInventoryObjectInfo WeaponObject
        {
            get
            {
                return this.weaponObject;
            }
        }
    }
}
