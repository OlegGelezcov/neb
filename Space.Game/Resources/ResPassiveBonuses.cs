using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class ResPassiveBonuses {
        public PassiveBonusData speed { get; private set; }
        public PassiveBonusData damageDron { get; private set; }
        public PassiveBonusData healDron { get; private set; }
        public PassiveBonusData critChance { get; private set; }
        public PassiveBonusData critDamage { get; private set; }
        public PassiveBonusData damage { get; private set; }
        public PassiveBonusData resist { get; private set; }
        public PassiveBonusData maxHP { get; private set; }
        public PassiveBonusData maxEnergy { get; private set; }
        public PassiveBonusData coloredLoot { get; private set; }
        public PassiveBonusData restoreHP { get; private set; }
        public PassiveBonusData restoreEnergy { get; private set; }
        public PassiveBonusData optimalDistance { get; private set; }
        public PassiveBonusData chanceDontDropLoot { get; private set; }
        public PassiveBonusData chanceCraftColoredModule { get; private set; }

        public PassiveBonusData[] allData {
            get {
                return mAllData;
            }
        }

        public float GetBaseTime(PassiveBonusType type) {
            foreach(var data in allData ) {
                if(data.type == type) {
                    return data.timeToFirstTier;
                }
            }
            return 0f;

        }

        public int GetElementsForFirstTier(PassiveBonusType type) {
            foreach(var data in allData) {
                if(data.type == type) {
                    return data.nebulaElementsForTier;
                }
            }
            return 0;
        }

        public PassiveBonusData GetData(PassiveBonusType type) {
            foreach(var data in allData ) {
                if(data.type == type) {
                    return data;
                }
            }
            return null;
        }

        private PassiveBonusData[] mAllData;


        public void Load(string basePath) {
            string fullPath = System.IO.Path.Combine(basePath, "Data/passive_bonuses.xml");
            XDocument document = XDocument.Load(fullPath);
            var passiveBonusesElement = document.Element("passive_bonuses");
            speed = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.Speed, passiveBonusesElement.Element("speed"));
            damageDron = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.DamageDron, passiveBonusesElement.Element("damage_dron"));
            healDron = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.HealDron, passiveBonusesElement.Element("heal_dron"));
            critChance = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.CritChance, passiveBonusesElement.Element("crit_chance"));
            critDamage = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.CritDamage, passiveBonusesElement.Element("crit_damage"));
            damage = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.Damage, passiveBonusesElement.Element("damage"));
            resist = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.Resist, passiveBonusesElement.Element("resist"));
            maxHP = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.MaxHP, passiveBonusesElement.Element("max_hp"));
            maxEnergy = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.MaxEnergy, passiveBonusesElement.Element("max_energy"));
            coloredLoot = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.ColoredLoot, passiveBonusesElement.Element("colored_loot"));
            restoreHP = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.RestoreHPSpeed, passiveBonusesElement.Element("restore_hp"));
            restoreEnergy = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.RestoreEnergy, passiveBonusesElement.Element("restore_energy"));
            optimalDistance = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.OptimalDistance, passiveBonusesElement.Element("optimal_distance"));
            chanceDontDropLoot = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.ChanceNotDropLootAtDeath, passiveBonusesElement.Element("chance_dont_drop_loot"));
            chanceCraftColoredModule = PassiveBonusData.CreateFromXml(Common.PassiveBonusType.ChanceCraftColoredModule, passiveBonusesElement.Element("chance_craft_colored_module"));
            mAllData = new PassiveBonusData[] {
                speed, damageDron, healDron,
                critChance, critDamage, damage,
                resist, maxHP, maxEnergy,
                coloredLoot, restoreHP, restoreEnergy,
                optimalDistance, chanceDontDropLoot, chanceCraftColoredModule
            };
        }
    }
}
