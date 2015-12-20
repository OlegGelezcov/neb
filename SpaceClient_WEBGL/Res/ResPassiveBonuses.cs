using Common;
using System.Collections.Generic;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class ResPassiveBonuses {
        public List<ResPassiveBonusData> bonuses { get; private set; }

        public void Load(string xml) {
            bonuses = new List<ResPassiveBonusData>();
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
            var parent = document.Element("passive_bonuses");
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.Speed, parent.Element("speed")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.DamageDron, parent.Element("damage_dron")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.HealDron, parent.Element("heal_dron")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.CritChance, parent.Element("crit_chance")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.CritDamage, parent.Element("crit_damage")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.Damage, parent.Element("damage")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.Resist, parent.Element("resist")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.MaxHP, parent.Element("max_hp")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.MaxEnergy, parent.Element("max_energy")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.ColoredLoot, parent.Element("colored_loot")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.RestoreHPSpeed, parent.Element("restore_hp")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.RestoreEnergy, parent.Element("restore_energy")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.OptimalDistance, parent.Element("optimal_distance")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.ChanceNotDropLootAtDeath, parent.Element("chance_dont_drop_loot")));
            bonuses.Add(new ResPassiveBonusData(Common.PassiveBonusType.ChanceCraftColoredModule, parent.Element("chance_craft_colored_module")));
        }

        public ResPassiveBonusData GetBonusData(PassiveBonusType type) {
            foreach(var bonus in bonuses) {
                if(bonus.type == type ) {
                    return bonus;
                }
            }
            return null;
        }
    }
}
