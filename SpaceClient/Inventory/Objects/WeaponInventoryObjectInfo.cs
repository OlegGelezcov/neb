using System;
using Common;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client.Inventory.Objects
{
    public class WeaponInventoryObjectInfo : IInventoryObjectInfo
    {
        private string id;
        private string template;
        private int level;
        private ObjectColor color;
        private WeaponDamageType damageType;

        public float damage { get; private set; }
        public float baseCritChance { get; private set; }
        private float optimalDistance;
        public Workshop workshop { get; private set; }
        public bool binded { get; private set; }




        public Hashtable GetInfo()
        {
            return rawHash;
        }




        public void ParseInfo(System.Collections.Hashtable info)
        {
            rawHash = info;
            this.id = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.template = info.GetValue<string>((int)SPC.Template, string.Empty);
            this.level = info.GetValue<int>((int)SPC.Level, 0);
            this.damage = info.Value<float>((int)SPC.Damage);

            this.optimalDistance = info.GetValue<float>((int)SPC.OptimalDistance, 0f);

            this.color = (ObjectColor)(byte)info.GetValue<int>((int)SPC.Color, (int)(byte)ObjectColor.white);
            this.damageType = (WeaponDamageType)(byte)info.Value<byte>((int)SPC.DamageType);
            this.baseCritChance = (float)info.GetValue<float>((int)SPC.CritChance, 0f);
            workshop = (Workshop)(byte)(int)info.GetValue<int>((int)SPC.Workshop, (int)Workshop.DarthTribe);
            binded = info.GetValue<bool>((int)SPC.Binded, binded);
        }

        public WeaponInventoryObjectInfo() { }

        public WeaponInventoryObjectInfo(Hashtable info) {
            ParseInfo(info);
        }

        public string Id
        {
            get
            {
                return this.id;
            }
        }

        public InventoryObjectType Type
        {
            get
            {
                return InventoryObjectType.Weapon;
            }
        }

        public string Template
        {
            get
            {
                return this.template;
            }
        }

        public int Level
        {
            get
            {
                return this.level;
            }
        }


        public float OptimalDistance
        {
            get
            {
                return this.optimalDistance;
            }
        }


        public ObjectColor Color
        {
            get
            {
                return this.color;
            }
        }

        public WeaponDamageType DamagetType {
            get {
                return damageType;
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

        public ObjectColor MyColor() {
            return this.Color;
        }

        public override string ToString()
        {
            return string.Format("id: {0}", id.Substring(0, 3));
        }

        public bool HasColor() {
            return true;
        }
    }
}
