using System;
using System.Collections;
using Common;
using System.IO;
using System.Xml.Linq;
using Space.Game.Resources;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Space.Game.Resources
{
    public class WeaponDropSettings : IResourceLoader
    {
        private bool loaded;
        private int m_MaxLevel;
        private int m_DamageOdPtMax;

        private ConcurrentDictionary<Workshop, WeaponWorkshopSetting> settings;

        public int maxLevel {
            get {
                return m_MaxLevel;
            }
        }

        public int damageOdPtMax {
            get {
                return m_DamageOdPtMax;
            }
        }

        public bool TryGetSetting(Workshop workshop, out WeaponWorkshopSetting setting) {
            return settings.TryGetValue(workshop, out setting);
        }

        public WeaponDropSettings()
        {
            this.loaded = false;
            settings = new ConcurrentDictionary<Workshop, WeaponWorkshopSetting>();        }

        public bool Load(string basePath)
        {
            var fullPath = Path.Combine(basePath, "Data/Drop/weapon_settings.xml");
            XDocument document = XDocument.Load(fullPath);
            settings = new ConcurrentDictionary<Workshop, WeaponWorkshopSetting>();

            var dmp1 = document.Element("settings").Elements("param").Select(pe => {
                switch(pe.GetString("name")) {
                    case "max_level":
                        m_MaxLevel = pe.GetInt("value");
                        break;
                    case "damage_od_pt_max":
                        m_DamageOdPtMax = pe.GetInt("value");
                        break;
                }
                return pe;
            }).ToList();

            var dump = document.Element("settings").Elements("workshop").Select(we => {
                WeaponWorkshopSetting wset = new WeaponWorkshopSetting(we);
                settings.TryAdd(wset.workshop, wset);
                return we;
            }).ToList();
            /*
            var lst = document.Element("settings").Elements("workshop").Select(e => {
                Workshop workshop = (Workshop)Enum.Parse(typeof(Workshop), e.GetString("name"));
                WeaponWorkshopSetting set = new WeaponWorkshopSetting {
                    workshop = workshop,
                    base_damage = e.Element("base_damage").GetFloat("value"),
                    base_optimal_distance = e.Element("base_optimal_distance").GetFloat("value"),
                    damage_points_value = e.Element("damage_points_value").GetFloat("value"),
                    optimal_distance_points_value = e.Element("optimal_distance_points_value").GetFloat("value"),
                    base_damage_factor = e.Element("base_damage_factor").GetFloat("value"),
                    base_optimal_distance_factor = e.Element("base_optimal_distance_factor").GetFloat("value"),
                    damage_points_factor = e.Element("damage_points_factor").GetFloat("value"),
                    optimal_distance_points_factor = e.Element("optimal_distance_points_factor").GetFloat("value"),
                    base_crit_chance = e.Element("base_crit_chance").GetFloat("value")
                };
                if(!settings.TryAdd(workshop, set)) {
                    throw new Exception("Error of adding WeaponWorkshopSetting to dictionary");
                }
                return workshop;
            }).ToList();
            */

            this.loaded = true;
            return this.loaded;
        }

        public bool Loaded
        {
            get { return this.loaded; }
        }

    }


    public class WeaponWorkshopSetting {
        /*
        public Workshop workshop;
        public float base_damage;
        public float base_optimal_distance;
        public float damage_points_value;
        public float optimal_distance_points_value;
        public float base_damage_factor;
        public float base_optimal_distance_factor;
        public float damage_points_factor;
        public float optimal_distance_points_factor;
        public float base_crit_chance;*/

        private Workshop m_Workshop;
        private BaseParam m_Damage;
        private BaseParam m_OptimalDistance;
        private AddParam m_CritChance;

        public WeaponWorkshopSetting(XElement element) {
            m_Workshop = (Workshop)System.Enum.Parse(typeof(Workshop), element.GetString("name"));
            m_Damage = new BaseParam(element.Element("damage"));
            m_OptimalDistance = new BaseParam(element.Element("od"));
            m_CritChance = new AddParam(element.Element("cc"));
        }

        public Workshop workshop {
            get {
                return m_Workshop;
            }
        }

        public BaseParam damage {
            get {
                return m_Damage;
            }
        }

        public BaseParam optimalDistance {
            get {
                return m_OptimalDistance;
            }
        }

        public AddParam critChance {
            get {
                return m_CritChance;
            }
        }
    }
}
