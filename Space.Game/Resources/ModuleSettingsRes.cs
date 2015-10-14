using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Collections;
using GameMath;
using System.Collections.Concurrent;

namespace Space.Game.Resources
{
    public class ModuleSettingsRes : IResourceLoader
    {
        private ConcurrentDictionary<Workshop, ModuleSettingData> settings = new ConcurrentDictionary<Workshop, ModuleSettingData>();
        private bool mLoaded = false;

        public bool Loaded {
            get {
                return mLoaded;
            }
        }

        public bool TeyGetWorkshopData(Workshop workshop, out ModuleSettingData data) {
            return settings.TryGetValue(workshop, out data);
        }

        public bool Load(string basePath)
        {
            string fullPath = Path.Combine(basePath, "Data/Drop/module_settings.xml");
            XDocument document = XDocument.Load(fullPath);
            settings = new ConcurrentDictionary<Workshop, ModuleSettingData>();

            var lst = document.Element("settings").Elements("workshop").Select(e => {
                Workshop workshop = (Workshop)Enum.Parse(typeof(Workshop), e.GetString("name"));
                ModuleSettingData data = new ModuleSettingData {
                    workshop = workshop,
                    base_hp = e.Element("base_hp").GetFloat("value"),
                    base_speed = e.Element("base_speed").GetFloat("value"),
                    base_cargo = e.Element("base_cargo").GetFloat("value"),
                    base_hp_factor = e.Element("base_hp_factor").GetFloat("value"),
                    base_speed_factor = e.Element("base_speed_factor").GetFloat("value"),
                    base_cargo_factor = e.Element("base_cargo_factor").GetFloat("value"),
                    resistance_aux_number = e.Element("resistance_aux_number").GetFloat("value"),
                    critical_chance_aux_number = e.Element("critical_chance_aux_number").GetFloat("value"),
                    bonus_aux_number = e.Element("bonus_aux_number").GetFloat("value"),
                    slot_settings = LoadSlotSettings(e.Element("slots"))
                };
                if(!settings.TryAdd(workshop, data)) {
                    throw new Exception("Error of adding ModuleSettingData to dictionary");
                }
                return workshop;
            }).ToList();
            mLoaded = true;
            return true;
        }

        private ConcurrentDictionary<ShipModelSlotType, ModuleSlotSettingData> LoadSlotSettings(XElement parent) {
            ConcurrentDictionary<ShipModelSlotType, ModuleSlotSettingData> result = new ConcurrentDictionary<ShipModelSlotType, ModuleSlotSettingData>();
            var lst = parent.Elements("slot").Select(e => {
                ShipModelSlotType slotType = (ShipModelSlotType)Enum.Parse(typeof(ShipModelSlotType), e.GetString("type"));
                ModuleSlotSettingData data = new ModuleSlotSettingData {
                    slotType = slotType,
                    hp_points_value = e.Element("hp_points_value").GetFloat("value"),
                    speed_points_value = e.Element("speed_points_value").GetFloat("value"),
                    cargo_points_value = e.Element("cargo_points_value").GetFloat("value"),
                    resistance_points_value = e.Element("resistance_points_value").GetFloat("value"),
                    critical_chance_points_value = e.Element("critical_chance_points_value").GetFloat("value"),
                    critical_damage_points_value = e.Element("critical_damage_points_value").GetFloat("value"),
                    damage_bonus_points_value = e.Element("damage_bonus_points_value").GetFloat("value"),
                    energy_bonus_points_value = e.Element("energy_bonus_points_value").GetFloat("value"),
                    cargo_bonus_points_value = e.Element("cargo_bonus_points_value").GetFloat("value"),
                    speed_bonus_points_value = e.Element("speed_bonus_points_value").GetFloat("value"),
                    hp_points_factor = e.Element("hp_points_factor").GetFloat("value"),
                    speed_points_factor = e.Element("speed_points_factor").GetFloat("value"),
                    resistance_points_factor = e.Element("resistance_points_factor").GetFloat("value"),
                    critical_chance_points_factor = e.Element("critical_chance_points_factor").GetFloat("value"),
                    critical_damage_points_factor = e.Element("critical_damage_points_factor").GetFloat("value"),
                    damage_bonus_points_factor = e.Element("damage_bonus_points_factor").GetFloat("value"),
                    energy_bonus_points_factor = e.Element("energy_bonus_points_factor").GetFloat("value"),
                    cargo_bonus_points_factor = e.Element("cargo_bonus_points_factor").GetFloat("value"),
                    speed_bonus_points_factor = e.Element("speed_bonus_points_factor").GetFloat("value")
                };
                if ( !result.TryAdd(slotType, data) ) {
                    throw new Exception("Error of adding ModuleSlotSettingData to dictionary");
                }
                return slotType;
            }).ToList();
            return result;
        }
    }

    public class ModuleSettingData {
        public Workshop workshop;
        public float base_hp;
        public float base_speed;
        public float base_cargo;
        public float base_hp_factor;
        public float base_speed_factor;
        public float base_cargo_factor;
        public float resistance_aux_number;
        public float critical_chance_aux_number;
        public float bonus_aux_number;
        public ConcurrentDictionary<ShipModelSlotType, ModuleSlotSettingData> slot_settings;

        public bool TryGetSlotSetting(ShipModelSlotType slotType, out ModuleSlotSettingData data) {
            return slot_settings.TryGetValue(slotType, out data);
        }
    }

    public class ModuleSlotSettingData {
        public ShipModelSlotType slotType;
        public float hp_points_value;
        public float speed_points_value;
        public float cargo_points_value;
        public float resistance_points_value;
        public float critical_chance_points_value;
        public float critical_damage_points_value;
        public float damage_bonus_points_value;
        public float energy_bonus_points_value;
        public float cargo_bonus_points_value;
        public float speed_bonus_points_value;
        public float hp_points_factor;
        public float speed_points_factor;
        public float resistance_points_factor;
        public float critical_chance_points_factor;
        public float critical_damage_points_factor;
        public float damage_bonus_points_factor;
        public float energy_bonus_points_factor;
        public float cargo_bonus_points_factor;
        public float speed_bonus_points_factor;
    }
}
