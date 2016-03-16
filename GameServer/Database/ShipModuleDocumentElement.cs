using Common;
using Space.Game.Ship;
using System.Collections.Generic;

namespace Space.Database {
    public class ShipModuleDocumentElement {
        public string ModuleId { get; set; } //
        public byte SlotType { get; set; }//
        public int Level { get; set; }//
        public string Name { get; set; }//
        public byte Workshop { get; set; }//
        public string TemplateId { get; set; }//
        public string Prefab { get; set; } //

        public float HP { get; set; }//
        public int Hold { get; set; }//
        public float Resist { get; set; }//
        public float Speed { get; set; }//
        public float DamageBonus { get; set; }//
        public int RepairSlots { get; set; }
        public float Weight { get; set; }
        public int Color { get; set; }//
        public int Skill { get; set; } //
        public string SetID { get; set; }//
        public float CritChance { get; set; } //
        public float CritDamage { get; set; }//

        public float energyBonus { get; set; } //
        public float speedBonus { get; set; }//
        public float holdBonus { get; set; }//
        public int difficulty { get; set; }

        public float rocketResist { get; set; }
        public float acidResist { get; set; }
        public float laserResist { get; set; }

        public Dictionary<string, int> CraftMaterials { get; set; }//

        public void Set(ShipModule baseModule) {
            if(baseModule == null ) {
                return;
            }

            this.ModuleId = baseModule.Id;
            this.SlotType = baseModule.SlotType.toByte();
            this.Level = baseModule.Level;
            this.Name = baseModule.Name;
            this.Workshop = baseModule.Workshop.toByte();
            this.TemplateId = baseModule.TemplateModuleId;
            this.Prefab = baseModule.Prefab;
            this.HP = baseModule.HP;
            this.Hold = baseModule.Hold;
            this.Resist = baseModule.commonResist;
            this.rocketResist = baseModule.rocketResist;
            this.acidResist = baseModule.acidResist;
            this.laserResist = baseModule.laserResist;

            this.Speed = baseModule.Speed;
            this.DamageBonus = baseModule.DamageBonus;
            this.Color = baseModule.Color.toByte();
            this.Skill = baseModule.Skill;
            this.SetID = baseModule.Set;
            this.CritChance = baseModule.CritChance;
            this.CritDamage = baseModule.CritDamage;
            this.CraftMaterials = baseModule.CraftMaterials;
            this.energyBonus = baseModule.EnergyBonus;
            this.speedBonus = baseModule.SpeedBonus;
            this.holdBonus = baseModule.HoldBonus;
            this.difficulty = (int)(byte)baseModule.Difficulty;
        }

        public ShipModule SourceObject() {
            if (this.CraftMaterials == null)
                this.CraftMaterials = new Dictionary<string, int>();

            ShipModule result = new ShipModule(
                (ShipModelSlotType)this.SlotType, 
                this.ModuleId, 
                this.Level, 
                this.Name, 
                (Workshop)this.Workshop, 
                this.TemplateId, 
                this.CraftMaterials, 
                Difficulty.none
            );

            result.SetPrefab(this.Prefab);
            result.SetHP(this.HP);
            result.SetHold(this.Hold);
            result.SetCommonResist(this.Resist);
            result.SetAcidResist(this.acidResist);
            result.SetLaserResist(this.laserResist);
            result.SetRocketResist(this.rocketResist);
            result.SetSpeed(this.Speed);
            result.SetDamageBonus(this.DamageBonus);
            result.SetColor((ObjectColor)this.Color);
            result.SetSkill(this.Skill);
            result.SetSet(this.SetID);
            result.SetCritChance(this.CritChance);
            result.SetCritDamage(this.CritDamage);
            result.SetEnergyBonus(this.energyBonus);
            result.SetSpeedBonus(this.speedBonus);
            result.SetHoldBonus(this.holdBonus);

            return result;
        }
    }
}
