using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Common;
using System.Collections;


namespace Space.Game.Resources {
    public class ServerInputsRes {
        public Hashtable Inputs { get; private set; }

        public float playerLootChestLifeTime { get; private set; }
        public float miningStationHP { get; private set; }

        public float standardOutpostSize { get; private set; }
        public float standardFortificationSize { get; private set; }
        public float standardTurretSize { get; private set; }
        public float standardOutpostHp { get; private set; }
        public float standardFortificationHp { get; private set; }
        public float standardTurretHp { get; private set; }

        public int pvpStoreMinLevel { get; private set; }


        /// <summary>
        /// Start player when he first entering to game
        /// </summary>
        public int startPlayerTime { get; private set; }

        /// <summary>
        /// Game time for using single pass
        /// </summary>
        public int timeForPass { get; private set; }

        public void Load(string basePath, string relativePath) {
            string fullPath = Path.Combine(basePath, relativePath);
            XDocument document = XDocument.Load(fullPath);
            this.Inputs = new Hashtable();
            var dump = document.Element("inputs").Elements("input").Select(e => {
                string key = e.Attribute("key").Value;
                string valStr = e.Attribute("value").Value;
                string type = e.Attribute("type").Value;
                this.Inputs.Add(key, CommonUtils.ParseValue(valStr, type));
                return key;
            }).ToList();
            playerLootChestLifeTime = Inputs.GetValue<float>("player_dead_chest_life_time", 0f);
            miningStationHP = Inputs.GetValue<float>("mining_station_hp", 0f);
            standardOutpostSize = Inputs.GetValue<float>("standard_outpost_size", 0f);
            standardFortificationSize = Inputs.GetValue<float>("standard_fortification_size", 0f);
            standardTurretSize = Inputs.GetValue<float>("standard_turret_size", 0f);
            standardOutpostHp = Inputs.GetValue<float>("standard_outpost_hp", 0f);
            standardFortificationHp = Inputs.GetValue<float>("standard_fortification_hp", 0f);
            standardTurretHp = Inputs.GetValue<float>("standard_turret_hp", 0f);
            startPlayerTime = Inputs.GetValue<int>("start_player_game_time", 0);
            timeForPass = Inputs.GetValue<int>("time_for_pass", 0);
            pvpStoreMinLevel = Inputs.GetValue<int>("pvp_store_min_level", 0);
        }

        public T GetValue<T>(string key) {
            if (this.Inputs.ContainsKey(key))
                return (T)this.Inputs[key];
            if (typeof(T) == typeof(float)) {
                return (T)(object)(0.0f);
            } else if (typeof(T) == typeof(int)) {
                return (T)(object)(0);
            } else if (typeof(T) == typeof(bool)) {
                return (T)(object)(false);
            } else if (typeof(T) == typeof(float[])) {
                return (T)(object)(new float[] { 0.0f, 0.0f, 0.0f });
            } else if (typeof(T) == typeof(string[])) {
                return (T)(object)(new string[] { });
            } else if (typeof(T) == typeof(string)) {
                return (T)(object)(string.Empty);
            }
            return default(T);
        }
    }
}
