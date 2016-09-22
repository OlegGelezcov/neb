using Common;
using Nebula.Drop;
using ServerClientCommon;
using System.Collections;

namespace Space.Game.Inventory.Objects {
    public class WeaponObject : IInventoryObject
    {

        private string id;
        private string template;
        private int level;
        private ObjectColor color;
        private WeaponDamageType damageType;
        private int mWorkshop;
        //public float damage { get; private set; }
        private readonly WeaponDamage m_Damage = new WeaponDamage();
        private float optimalDistance;
        public float baseCritChance { get; private set; }
        private Hashtable mRaw;
        public bool binded { get; private set; } = false;

        public void Bind() { binded = true; }

        public WeaponObject(Hashtable info)
        {
            this.ParseInfo(info);
        }

        public WeaponObject(
            string id, 
            string template, 
            int level, 
            //float damage, 
            WeaponDamage damage,
            float optimalDistance,
            ObjectColor color,
            WeaponDamageType damageType,
            float inBaseCritChance,
            int workshop)
        {
            this.id = id;
            this.template = template;
            this.level = level;
            //this.damage = damage;
            m_Damage.SetFromDamage(damage);
            this.optimalDistance = optimalDistance;
            this.color = color;
            this.damageType = damageType;
            this.baseCritChance = inBaseCritChance;
            this.mWorkshop = workshop;
            isNew = true;

            mRaw = GetInfo();
        }

        public Hashtable GetInfo()
        {
            return new Hashtable 
            {
                {(int)SPC.Id, this.id },
                {(int)SPC.Template, this.template },
                {(int)SPC.Level, this.level },
                {(int)SPC.Damage, m_Damage.totalDamage },
                {(int)SPC.OptimalDistance, this.optimalDistance },
                {(int)SPC.Color, (int)(byte)this.color },
                {(int)SPC.ItemType, (int)(byte)this.Type },
                {(int)SPC.DamageType, (int)(byte)this.damageType },
                {(int)SPC.PlacingType, placingType },
                {(int)SPC.CritChance, baseCritChance },
                {(int)SPC.Workshop, mWorkshop },
                {(int)SPC.Binded, binded },
                {(int)SPC.Splittable, splittable },
                {(int)SPC.IsNew, isNew},
                {(int)SPC.RocketDamage, rocketDamage },
                {(int)SPC.LaserDamage, laserDamage },
                {(int)SPC.AcidDamage, acidDamage },
                {(int)SPC.WeaponBaseType, (int)baseType }
            };
        }

        public WeaponObject(WeaponGenList gen) {
            this.id = gen.id;
            this.template = gen.template;
            this.level = gen.level;
            this.optimalDistance = gen.optimalDistance;
            this.color = gen.color;
            this.damageType = WeaponDamageType.damage;
            this.baseCritChance = gen.critChance;
            this.mWorkshop = (byte)gen.workshop;
            this.binded = false;
            this.isNew = false;
            m_Damage.SetRocketDamage(gen.rocketDamage);
            m_Damage.SetLaserDamage(gen.laserDamage);
            m_Damage.SetAcidDamage(gen.acidDamage);
            m_Damage.SetBaseType(gen.baseType);
            mRaw = GetInfo();
        }

        public void ParseInfo(Hashtable info)
        {
            mRaw = info;
            this.id = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.template = info.GetValue<string>((int)SPC.Template, string.Empty);
            this.level = info.GetValue<int>((int)SPC.Level, 0);
            //this.damage = info.Value<float>((int)SPC.Damage);
            this.optimalDistance = info.GetValue<float>((int)SPC.OptimalDistance, 0f);
            this.color = (ObjectColor)(byte)info.GetValue<int>((int)SPC.Color, (int)(byte)ObjectColor.white);
            this.damageType = (WeaponDamageType)(byte)info.GetValue<int>((int)SPC.DamageType, 0);
            this.baseCritChance = info.GetValue<float>((int)SPC.CritChance, 0f);
            this.mWorkshop = info.GetValue<int>((int)SPC.Workshop, (int)(byte)Workshop.DarthTribe);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
            isNew = info.GetValue<bool>((int)SPC.IsNew, false);

            m_Damage.SetRocketDamage(info.GetValue<float>((int)SPC.RocketDamage, 0f));
            m_Damage.SetLaserDamage(info.GetValue<float>((int)SPC.LaserDamage, 0f));
            m_Damage.SetAcidDamage(info.GetValue<float>((int)SPC.AcidDamage, 0f));

            int iBaseType = info.GetValue<int>((int)SPC.WeaponBaseType, (int)WeaponBaseType.None);
            WeaponBaseType wbt = (WeaponBaseType)iBaseType;
            if(wbt == WeaponBaseType.None ) {
                wbt = WeaponBaseType.Laser;
            }
            m_Damage.SetBaseType(wbt);
        }

        public WeaponBaseType baseType {
            get {
                return m_Damage.baseType;
            }
        }

        private float rocketDamage {
            get {
                return m_Damage.rocketDamage;
            }
        }
        private float acidDamage {
            get {
                return m_Damage.acidDamage;
            }
        }
        private float laserDamage {
            get {
                return m_Damage.laserDamage;
            }
        }

        public WeaponDamage damage {
            get {
                return m_Damage;
            }
        }

        public bool isNew {
            get;
            private set;
        }
        public void ResetNew() {
            isNew = false;
        }
        public void SetNew(bool val) {
            isNew = val;
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
