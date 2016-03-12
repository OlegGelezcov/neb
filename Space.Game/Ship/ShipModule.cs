namespace Space.Game.Ship
{
    using Common;
    using Space.Game.Drop;
    using System.Collections;
    using System.Text;
    using System.Collections.Generic;
    using ServerClientCommon;
    using System;
    using Space.Game.Inventory;

    public class ShipModule : IDroppable, IInventoryObject, IInfo
    {
        private  string id;                                     //1
        private  ShipModelSlotType slotType;                    //2
        private  int level;                                     //3
        private  string name;                                   //4
        private  Workshop workshop;                             //5 
        private  string templateModuleId;                       //6
        private ObjectColor color;                              //14

        //base parameters
        private float hp;                                       //8
        private float speed;                                    //11
        private int hold;                                       //9


        private float resist;                                   //10      
        private float damageBonus;                              //12
        private float energyBonus;       
        private float critChance;                               //17
        private float critDamage;                               //18
        private float speedBonus;
        private float holdBonus;

        private Dictionary<string, int> craftMaterials;         //21
        private Difficulty difficulty;                          //22
        private string prefab = "";                             //23


        private int skillId;                                    //15
        private string set;                                     //16

        private Hashtable mRaw;
        public bool binded { get; private set; } = false;

        public void Bind() { binded = true; }

        public Hashtable GetInfo() 
        {
            Hashtable info = new Hashtable();
            //base
            info.Add((int)SPC.Id, id);                                                                 //1
            info.Add((int)SPC.Type, slotType.toByte());                                                //2
            info.Add((int)SPC.Level, level);                                                           //3
            info.Add((int)SPC.Name, name);                                                             //4
            info.Add((int)SPC.Workshop, (int)workshop.toByte());                                            //5
            info.Add((int)SPC.Template, this.templateModuleId);                                      //15
            info.Add((int)SPC.Color, (int)color.toByte());                                                  //14
            info.Add((int)SPC.HoldType, (int)this.Type.toByte());                                          //17
            info.Add((int)SPC.PlacingType, placingType);
            info.Add((int)SPC.ItemType, (int)(byte)Type);

            //base
            info.Add((int)SPC.Health, hp);                                                                 //8
            info.Add((int)SPC.Speed, speed);                                                           //9          
            info.Add((int)SPC.Hold, hold);                                                             //11


            info.Add((int)SPC.Resist, resist);                                                         //10           
            info.Add((int)SPC.DamageBonus, damageBonus);                                              //12
            info.Add((int)SPC.EnergyBonus, energyBonus);
            info.Add((int)SPC.CritChance, this.critChance);                                           //18
            info.Add((int)SPC.CritDamage, this.critDamage);                                           //19
            info.Add((int)SPC.SpeedBonus, speedBonus);
            info.Add((int)SPC.HoldBonus, holdBonus);

            info.Add((int)SPC.CraftMaterials, this.craftMaterials.toHash<string, int>());             //20
            info.Add((int)SPC.Difficulty, (int)this.difficulty.toByte());                              //23
            info.Add((int)SPC.Prefab, this.prefab);                                                    //24
            info.Add((int)SPC.Skill, this.skillId);                                                    //16
            info.Add((int)SPC.Set, set);
            info.Add((int)SPC.Binded, binded);
            info.Add((int)SPC.Splittable, splittable);
            info.Add((int)SPC.IsNew, isNew);                    

            return info;
        }

        public void ParseInfo(Hashtable info )
        {
            mRaw = info;

            this.id = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.slotType = (ShipModelSlotType)(byte)info.GetValue<int>((int)SPC.Type, (int)ShipModelSlotType.CB.toByte());
            this.level = info.GetValue<int>((int)SPC.Level, 0);
            this.name = info.GetValue<string>((int)SPC.Name, string.Empty);
            this.workshop = (Workshop)(byte)info.GetValue<int>((int)SPC.Workshop, (int)Workshop.DarthTribe.toByte());
            this.templateModuleId = info.GetValue<string>((int)SPC.Template, string.Empty);
            this.color = (ObjectColor)(int)info.GetValue<int>((int)SPC.Color, (int)ObjectColor.white.toByte());

            this.hp = info.GetValue<float>((int)SPC.Health, 0);
            this.speed = info.GetValue<float>((int)SPC.Speed, 0);
            this.hold = info.GetValue<int>((int)SPC.Hold, 0);

            this.resist = info.GetValue<float>((int)SPC.Resist, 0);
            this.damageBonus = info.GetValue<float>((int)SPC.DamageBonus, 0);
            energyBonus = info.GetValue<float>((int)SPC.EnergyBonus, 0);
            this.critChance = info.GetValue<float>((int)SPC.CritChance, 0);
            this.critDamage = info.GetValue<float>((int)SPC.CritDamage, 0);
            speedBonus = info.GetValue<float>((int)SPC.SpeedBonus, 0);
            holdBonus = info.GetValue<float>((int)SPC.HoldBonus, 0);

            Hashtable craftMaterialsHash = info.GetValue<Hashtable>((int)SPC.CraftMaterials, new Hashtable());
            this.craftMaterials = craftMaterialsHash.toDict<string, int>();

            this.difficulty = (Difficulty)(byte)info.GetValue<int>((int)SPC.Difficulty, 0);
            this.prefab = info.GetValue<string>((int)SPC.Prefab, string.Empty);
            this.skillId = info.GetValue<int>((int)SPC.Skill, -1);
            this.set = info.GetValue<string>((int)SPC.Set, string.Empty);
            binded = info.GetValue<bool>((int)SPC.Binded, binded);
            isNew = info.GetValue<bool>((int)SPC.IsNew, isNew);
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
        public float EnergyBonus {
            get {
                return energyBonus;
            }
        }

        public void SetEnergyBonus(float e) { energyBonus = e; }
        public float SpeedBonus { get { return speedBonus; } }
        public void SetSpeedBonus(float s) { speedBonus = s; }
        public float HoldBonus {
            get {
                return holdBonus;
            }
        }
        public void SetHoldBonus(float h) {
            holdBonus = h;
        }

        public ShipModule(Hashtable info )
        {
            this.ParseInfo(info);
        }
        
        public ShipModule(ShipModelSlotType slotType, string id, int level, string name, 
            Workshop workshop, string templateModuleId, Dictionary<string, int> craftMaterials, Difficulty difficulty) 
        {
            this.slotType = slotType;
            this.id = id;
            this.level = level;
            this.name = name;
            this.workshop = workshop;
            this.templateModuleId = templateModuleId;
            this.craftMaterials = craftMaterials;
            this.difficulty = difficulty;
            isNew = true;
        }

        public static ShipModule Null()
        {
            return new ShipModule(ShipModelSlotType.CB, string.Empty, 0, string.Empty,
                Workshop.DarthTribe, string.Empty, new Dictionary<string, int>(), Difficulty.none);
        }

        public bool IsNull()
        {
            return (
                this.SlotType == ShipModelSlotType.CB &&
                string.IsNullOrEmpty(this.Id) &&
                this.Level == 0 &&
                string.IsNullOrEmpty(this.Name) &&
                this.Workshop == Common.Workshop.DarthTribe &&
                string.IsNullOrEmpty(this.TemplateModuleId) &&
                this.CraftMaterials.Count == 0 &&
                this.Difficulty == Common.Difficulty.none
                );
        }

        public void SetPrefab(string prefab)
        {
            this.prefab = prefab;
        }

        public string Prefab
        {
            get { return this.prefab; }
        }

        public void SetHP(float hp) {
            this.hp = hp;
        }
        public void SetHold(int hold) {
            this.hold = hold;
        }
        public void SetResist(float resist) {
            this.resist = resist;
        }
        public void SetSpeed(float speed) {
            this.speed = speed;
        }
        public void SetDamageBonus(float bonus) {
            this.damageBonus = bonus;
        }
        public void SetSet(string setId )
        {
            this.set = setId;
        }

        public void SetSkill(int id)
        {
            this.skillId = id;
        }

        public void SetColor(ObjectColor color) {
            this.color = color;
        }

        public void SetCritChance(float critChance)
        {
            this.critChance = critChance;
        }

        public void SetCritDamage(float critDamage)
        {
            this.critDamage = critDamage;
        }

        public void SetCraftMaterials(Dictionary<string, int> craftMaterials )
        {
            this.craftMaterials = craftMaterials;
        }




        public string Id {
            get {
                return id;
            }
        }

        public int Level {
            get {
                return level;
            }
        }

        public ShipModelSlotType SlotType {
            get {
                return slotType;
            }
        }




        public float HP {
            get {
                return hp;
            }
        }

        public int Hold {
            get {
                return hold;
            }
        }

        public float Resist {
            get {
                return resist;
            }
        }

        public float Speed {
            get {
                return speed;
            }
        }

        public float DamageBonus {
            get {
                return damageBonus;
            }
        }


        public string Name {
            get {
                return name;
            }
        }

        public Workshop Workshop {
            get {
                return workshop;
            }
        }

        public string Set {
            get {
                return set;
            }
        }

        public bool IsBelongToSet
        {
            get
            {
                return (false == string.IsNullOrEmpty(set));
            }
        }

        public ObjectColor Color {
            get {
                return color;
            }
        }

        public string TemplateModuleId
        {
            get
            {
                return this.templateModuleId;
            }
        }


        public int Skill
        {
            get
            {
                return skillId;
            }
        }

        public bool HasSkill
        {
            get
            {
                return (skillId != -1);
            }
        }

        public float CritChance
        {
            get
            {
                return this.critChance;
            }
        }

        public float CritDamage
        {
            get
            {
                return this.critDamage;
            }
        }

        public Dictionary<string, int> CraftMaterials
        {
            get
            {
                return this.craftMaterials;
            }
        }


        public override string ToString()
        {
            /*
            var craftSB = new StringBuilder();
            CommonUtils.ConstructHashString(this.craftMaterials.toHash<string, int>(), 1, ref craftSB);

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("id: {0}  color: {1}", id, color));
            sb.AppendLine(string.Format("type: {0} level: {1} name: {2} workshop: {3} set: {4}", slotType, level, name, workshop, set));
            sb.AppendLine(string.Format("slot count: {0}", weaponSlotsCount));
            sb.AppendLine(string.Format("hp: {0}  speed: {1}", hp, speed));
            sb.AppendLine(string.Format("skill: {0}", HasSkill ? this.skillId : "NO SKILL"));
            sb.AppendLine(string.Format("resist: {0}  hold: {1}", resist, hold));
            sb.AppendLine(string.Format("damage: {0}  distance: {1}", damageBonus,distanceBonus));
            sb.AppendLine(string.Format("Crit chance: {0}, crit damage: {1}", this.critChance, this.critDamage));
            sb.AppendLine("energy: {0}, energy restoration: {1}".f(this.energy, this.energyRestoration));
            sb.AppendLine("craft materials: {0}".f(craftSB.ToString()));
            sb.AppendLine("difficulty: {0}".f(this.difficulty));
            sb.AppendLine("------------------------------");
            return sb.ToString();
            */
            var sb = new StringBuilder();
            CommonUtils.ConstructHashString(this.GetInfo(), 1, ref sb);
            return sb.ToString();
        }


        public InventoryObjectType Type
        {
            get { return InventoryObjectType.Module; }
        }

        public Difficulty Difficulty
        {
            get
            {
                return this.difficulty;
            }
        }

        public int placingType {
            get {
                return (int)PlacingType.Station;
            }
        }

        public Hashtable rawHash {
            get {
                if(mRaw == null ) {
                    mRaw = GetInfo();
                }
                return mRaw;
            }
        }

        public bool splittable {
            get {
                return false;
            }
        }
    }
}
