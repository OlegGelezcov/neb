using System;
using Common;
using Space.Game.Drop;
using System.Collections;
using ServerClientCommon;

namespace Space.Game.Inventory.Objects
{
    public class WeaponObject : IInventoryObject
    {

        private string id;
        private string template;
        private int level;
        private ObjectColor color;
        private WeaponDamageType damageType;
        private int mWorkshop;
        public float damage { get; private set; }
        private float optimalDistance;
        public float baseCritChance { get; private set; }
        private Hashtable mRaw;
        public bool binded { get; private set; } = false;

        public void Bind() { binded = true; }

        public WeaponObject(Hashtable info)
        {
            this.ParseInfo(info);
        }

        public WeaponObject(string id, string template, int level, 
            float damage, 
            float optimalDistance,
            ObjectColor color,
            WeaponDamageType damageType,
            float inBaseCritChance,
            int workshop)
        {
            this.id = id;
            this.template = template;
            this.level = level;
            this.damage = damage;
            this.optimalDistance = optimalDistance;
            this.color = color;
            this.damageType = damageType;
            this.baseCritChance = inBaseCritChance;
            this.mWorkshop = workshop;

            mRaw = GetInfo();
        }

        public Hashtable GetInfo()
        {
            return new Hashtable 
            {
                {(int)SPC.Id, this.id },
                {(int)SPC.Template, this.template },
                {(int)SPC.Level, this.level },
                {(int)SPC.Damage, this.damage },
                {(int)SPC.OptimalDistance, this.optimalDistance },
                {(int)SPC.Color, (int)(byte)this.color },
                {(int)SPC.ItemType, (int)(byte)this.Type },
                {(int)SPC.DamageType, (int)(byte)this.damageType },
                {(int)SPC.PlacingType, placingType },
                {(int)SPC.CritChance, baseCritChance },
                {(int)SPC.Workshop, mWorkshop },
                {(int)SPC.Binded, binded },
                {(int)SPC.Splittable, splittable }
            };
        }

        public void ParseInfo(Hashtable info)
        {
            mRaw = info;
            this.id = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.template = info.GetValue<string>((int)SPC.Template, string.Empty);
            this.level = info.GetValue<int>((int)SPC.Level, 0);
            this.damage = info.Value<float>((int)SPC.Damage);
            this.optimalDistance = info.GetValue<float>((int)SPC.OptimalDistance, 0f);
            this.color = (ObjectColor)(byte)info.GetValue<int>((int)SPC.Color, (int)(byte)ObjectColor.white);
            this.damageType = (WeaponDamageType)(byte)info.GetValue<int>((int)SPC.DamageType, 0);
            this.baseCritChance = info.GetValue<float>((int)SPC.CritChance, 0f);
            this.mWorkshop = info.GetValue<int>((int)SPC.Workshop, (int)(byte)Workshop.DarthTribe);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
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

        public WeaponDamageType DamageType
        {
            get
            {
                return this.damageType;
            }
        }

        public Workshop workshop {
            get {
                return (Workshop)(byte)mWorkshop;
            }
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable rawHash {
            get {
                return mRaw;
            }
        }

        public bool splittable {
            get {
                return false;
            }
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            CommonUtils.ConstructHashString(GetInfo(), 1, ref sb);
            return sb.ToString();
        }
    }
}
