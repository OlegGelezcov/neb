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
        private int m_MaxLevel;
        private int m_HpSpeedCargoPtMax;
        private int m_AddPointMax;

        public int maxLevel {
            get {
                return m_MaxLevel;
            }
        }

        public int hpSpeedCargoPtMax {
            get {
                return m_HpSpeedCargoPtMax;
            }
        }

        public int addPointMax {
            get {
                return m_AddPointMax;
            }
        }

        public bool Loaded {
            get {
                return mLoaded;
            }
        }

        public bool TeyGetWorkshopData(Workshop workshop, out ModuleSettingData data) {
            return settings.TryGetValue(workshop, out data);
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("max level: {0}", maxLevel));
            builder.AppendLine(string.Format("max hp-speed-cargo point: {0}", hpSpeedCargoPtMax));
            builder.AppendLine(string.Format("add point max: {0}", addPointMax));
            foreach(var kvp in settings ) {
                builder.AppendLine(kvp.Value.ToString());
            }
            return builder.ToString();
        }

        public bool Load(string basePath)
        {
            string fullPath = Path.Combine(basePath, "Data/Drop/module_settings.xml");
            XDocument document = XDocument.Load(fullPath);
            settings = new ConcurrentDictionary<Workshop, ModuleSettingData>();

            XElement settingsElement = document.Element("settings");

            var dmp1 = settingsElement.Elements("param").Select(e => {
                switch(e.GetString("name")) {
                    case "max_level":
                        m_MaxLevel = e.GetInt("value");
                        break;
                    case "hp_speed_cargo_pt_max":
                        m_HpSpeedCargoPtMax = e.GetInt("value");
                        break;
                    case "add_point_max":
                        m_AddPointMax = e.GetInt("value");
                        break;
                }
                return e;
            }).ToList();

            var dump = settingsElement.Elements("workshop").Select(we => {
                ModuleSettingData data = new ModuleSettingData(we);
                settings.TryAdd(data.workshop, data);
                return data;
            }).ToList();

            /*
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
                    //resistance_aux_number = e.Element("resistance_aux_number").GetFloat("value"),
                    critical_chance_aux_number = e.Element("critical_chance_aux_number").GetFloat("value"),
                    bonus_aux_number = e.Element("bonus_aux_number").GetFloat("value"),
                    slot_settings = LoadSlotSettings(e.Element("slots"))
                };
                if(!settings.TryAdd(workshop, data)) {
                    throw new Exception("Error of adding ModuleSettingData to dictionary");
                }
                return workshop;
            }).ToList();*/

            mLoaded = true;
            return true;
        }

        /*
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
                    //resistance_points_factor = e.Element("resistance_points_factor").GetFloat("value"),
                    critical_chance_points_factor = e.Element("critical_chance_points_factor").GetFloat("value"),
                    critical_damage_points_factor = e.Element("critical_damage_points_factor").GetFloat("value"),
                    damage_bonus_points_factor = e.Element("damage_bonus_points_factor").GetFloat("value"),
                    energy_bonus_points_factor = e.Element("energy_bonus_points_factor").GetFloat("value"),
                    cargo_bonus_points_factor = e.Element("cargo_bonus_points_factor").GetFloat("value"),
                    speed_bonus_points_factor = e.Element("speed_bonus_points_factor").GetFloat("value"),
                    resist_max = e.Element("resist_max").GetFloat("value")
                };
                if ( !result.TryAdd(slotType, data) ) {
                    throw new Exception("Error of adding ModuleSlotSettingData to dictionary");
                }
                return slotType;
            }).ToList();
            return result;
        }*/
    }

    public class ModuleSettingData {
        private  Workshop m_Workshop;

        /*
        public float base_hp;
        public float base_speed;
        public float base_cargo;
        public float base_hp_factor;
        public float base_speed_factor;
        public float base_cargo_factor;
        //public float resistance_aux_number;
        public float critical_chance_aux_number;
        public float bonus_aux_number;*/

        private ConcurrentDictionary<ShipModelSlotType, ModuleSlotSettingData> m_SlotSettings;

        public bool TryGetSlotSetting(ShipModelSlotType slotType, out ModuleSlotSettingData data) {
            return m_SlotSettings.TryGetValue(slotType, out data);
        }

        public ModuleSettingData(XElement element) {
            m_SlotSettings = new ConcurrentDictionary<ShipModelSlotType, ModuleSlotSettingData>();
            m_Workshop = (Workshop)Enum.Parse(typeof(Workshop), element.GetString("name"));
            var dump = element.Elements("slot").Select(slotElement => {
                ModuleSlotSettingData slotData = new ModuleSlotSettingData(slotElement);
                m_SlotSettings.TryAdd(slotData.slotType, slotData);
                return slotData;
            }).ToList();
        }

        public Workshop workshop {
            get {
                return m_Workshop;
            }
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("workshop: " + workshop.ToString());
            foreach(var kvp in m_SlotSettings ) {
                builder.AppendLine(kvp.Value.ToString());
            }
            return builder.ToString();
        }
    }

    public class ModuleSlotSettingData {

        private ShipModelSlotType m_SlotType;

        private BaseParam m_Hp;
        private BaseParam m_Speed;
        private BaseParam m_Cargo;

        private AddParam m_Resist;
        private AddParam m_DamageBonus;
        private AddParam m_CargoBonus;
        private AddParam m_EnergyBonus;
        private AddParam m_SpeedBonus;
        private AddParam m_CritChanceBonus;
        private AddParam m_CritDamageBonus;

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("slot type: " + slotType.ToString() + "--------->");
            sb.AppendLine("hp: " + hp.ToString());
            sb.AppendLine("speed: " + speed.ToString());
            sb.AppendLine("cargo: " + cargo.ToString());
            sb.AppendLine("resist: " + resist.ToString());
            sb.AppendLine("damage bonus: " + damageBonus.ToString());
            sb.AppendLine("cargo bonus: " + cargoBonus.ToString());
            sb.AppendLine("energy bonus: " + energyBonus.ToString());
            sb.AppendLine("speed bonus: " + speedBonus.ToString());
            sb.AppendLine("crit chance: " + critChanceBonus.ToString());
            sb.AppendLine("crit damage bonus: " + critDamageBonus.ToString());
            return sb.ToString();
        }

        public ModuleSlotSettingData(XElement element) {
            m_SlotType = (ShipModelSlotType)Enum.Parse(typeof(ShipModelSlotType), element.GetString("type"));
            m_Hp = new BaseParam(element.Element("hp"));
            m_Speed = new BaseParam(element.Element("speed"));
            m_Cargo = new BaseParam(element.Element("cargo"));
            m_Resist = new AddParam(element.Element("resist"));
            m_DamageBonus = new AddParam(element.Element("damage_bonus"));
            m_CargoBonus = new AddParam(element.Element("cargo_bonus"));
            m_EnergyBonus = new AddParam(element.Element("energy_bonus"));
            m_SpeedBonus = new AddParam(element.Element("speed_bonus"));
            m_CritChanceBonus = new AddParam(element.Element("crit_chance"));
            m_CritDamageBonus = new AddParam(element.Element("crit_damage"));
        }

        public ShipModelSlotType slotType {
            get {
                return m_SlotType;
            }
        }

        public AddParam critDamageBonus {
            get {
                return m_CritDamageBonus;
            }
        }

        public AddParam critChanceBonus {
            get {
                return m_CritChanceBonus;
            }
        }

        public AddParam speedBonus {
            get {
                return m_SpeedBonus;
            }
        }
        public AddParam energyBonus {
            get {
                return m_EnergyBonus;
            }
        }
        public AddParam cargoBonus {
            get {
                return m_CargoBonus;
            }
        }
        public AddParam damageBonus {
            get {
                return m_DamageBonus;
            }
        }
        public AddParam resist {
            get {
                return m_Resist;
            }
        }
        public BaseParam cargo {
            get {
                return m_Cargo;
            }
        }
        public BaseParam speed {
            get {
                return m_Speed;
            }
        }
        public BaseParam hp {
            get {
                return m_Hp;
            }
        }

        /*
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
        //public float resistance_points_factor;
        public float critical_chance_points_factor;
        public float critical_damage_points_factor;
        public float damage_bonus_points_factor;
        public float energy_bonus_points_factor;
        public float cargo_bonus_points_factor;
        public float speed_bonus_points_factor;
        public float resist_max;*/
    }

    public class AddParam {
        private float m_Max;
        private float m_RndMin;
        private float m_RndMax;

        public override string ToString() {
            return string.Format("max: {0}, rnd min: {1}, rnd max: {2}",
                max, randMin, randMax);
        }

        public AddParam(XElement element) {
            var dump = element.Elements("param").Select(pe => {
                switch (pe.GetString("name")) {
                    case "max":
                        m_Max = pe.GetFloat("value");
                        break;
                    case "rnd_min":
                        m_RndMin = pe.GetFloat("value");
                        break;
                    case "rnd_max":
                        m_RndMax = pe.GetFloat("value");
                        break;
                }
                return pe;
            }).ToList();
        }

        public float max {
            get {
                return m_Max;
            }
        }
        public float randMin {
            get {
                return m_RndMin;
            }
        }
        public float randMax {
            get {
                return m_RndMax;
            }
        }
    }

    public class BaseParam {
        private float m_Base;
        private float m_LevelMult;
        private float m_PointMult;
        private float m_RndMin;
        private float m_RndMax;

        public override string ToString() {
            return string.Format("base: {0}, level mult: {1}, point mult: {2}, rnd min: {3}, rnd max: {4}",
                baseVal, levelMult, pointMult, randMin, randMax);
        }

        public BaseParam(XElement element) {
            /*
            m_Base = element.GetFloat("base");
            m_LevelMult = element.GetFloat("lvl_mult");
            m_PointMult = element.GetFloat("pt_mult");
            m_RndMin = element.GetFloat("rnd_min");
            m_RndMax = element.GetFloat("rnd_max");*/
            var dump = element.Elements("param").Select(pe => {
                switch (pe.GetString("name")) {
                    case "base":
                        m_Base = pe.GetFloat("value");
                        break;
                    case "lvl_mult":
                        m_LevelMult = pe.GetFloat("value");
                        break;
                    case "pt_mult":
                        m_PointMult = pe.GetFloat("value");
                        break;
                    case "rnd_min":
                        m_RndMin = pe.GetFloat("value");
                        break;
                    case "rnd_max":
                        m_RndMax = pe.GetFloat("value");
                        break;
                }
                return pe;
            }).ToList();
        }

        public float baseVal {
            get {
                return m_Base;
            }
        }

        public float levelMult {
            get {
                return m_LevelMult;
            }
        }

        public float pointMult {
            get {
                return m_PointMult;
            }
        }

        public float randMin {
            get {
                return m_RndMin;
            }
        }

        public float randMax {
            get {
                return m_RndMax;
            }
        }
    }

}
