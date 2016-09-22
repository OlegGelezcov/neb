using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class SpecialBotShipComponentData : BotShipComponentData {

        public ConcurrentDictionary<ShipModelSlotType, ModuleGenList> moduleList { get; private set; }

        public SpecialBotShipComponentData(XElement element) : base(element) {
            var modulesElement = element.Element("modules");
            moduleList = new ConcurrentDictionary<ShipModelSlotType, ModuleGenList>();
            var dump = modulesElement.Elements("module").Select(me => {
                ShipModelSlotType slot = (ShipModelSlotType)Enum.Parse(typeof(ShipModelSlotType), me.GetString("slot"));
                string id = me.GetString("id");
                int level = me.GetInt("level");
                string name = me.GetString("name");
                Workshop workshop = (Workshop)Enum.Parse(typeof(Workshop), me.GetString("workshop"));
                string dataId = me.GetString("data_id");
                ObjectColor color = (ObjectColor)Enum.Parse(typeof(ObjectColor), me.GetString("color"));
                string model = me.GetString("model");
                float hp = me.GetFloat("hp");
                float speed = me.GetFloat("speed");
                int hold = me.GetInt("hold");
                float critDamage = me.GetFloat("crit_damage");
                float resist = me.GetFloat("resist");
                float damageBonus = me.GetFloat("damage_bonus");
                float energyBonus = me.GetFloat("energy_bonus");
                float critChance = me.GetFloat("crit_chance");
                float speedBonus = me.GetFloat("speed_bonus");
                float holdBonus = me.GetFloat("hold_bonus");
                int skill = me.GetInt("skill");
                string setId = me.GetString("set_id");

                List<DeconstructItem> dItems = me.Element("ditems").Elements("item").Select(die => {
                    return new DeconstructItem {
                        id = die.GetString("id"),
                        count = die.GetInt("count")
                    };
                }).ToList();

                ModuleGenList gen = new ModuleGenList {
                    id = id,
                    slot = slot,
                    level = level,
                    name = name,
                    workshop = workshop,
                    dataId = dataId,
                    deconstructOre = dItems,
                    difficulty = difficulty,
                    color = color,
                    model = model,
                    hp = hp,
                    speed = speed,
                    hold = hold,
                    critDamage = critDamage,
                    resist = resist,
                    damageBonus = damageBonus,
                    energyBonus = energyBonus,
                    critChance = critChance,
                    speedBonus = speedBonus,
                    holdBonus = holdBonus,
                    skill = skill,
                    setId = setId
                };
                moduleList.TryAdd(gen.slot, gen);
                return gen;
            }).ToList();
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.special_ship_bot;
            }
        }
    }
}
