using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Common;
using System.Collections;


namespace Space.Game.Resources
{
    public class ServerInputsRes
    {
        public Hashtable Inputs { get; private set; }

        public float playerLootChestLifeTime { get; private set; }
        public float miningStationHP { get; private set; }

        public void Load(string basePath)
        {
            string fullPath = Path.Combine(basePath, "Data/server_inputs.xml");
            XDocument document = XDocument.Load(fullPath);
            this.Inputs = new Hashtable();
            var dump = document.Element("inputs").Elements("input").Select(e =>
            {
                string key = e.Attribute("key").Value;
                string valStr = e.Attribute("value").Value;
                string type = e.Attribute("type").Value;
                this.Inputs.Add(key, CommonUtils.ParseValue(valStr, type));
                return key;
            }).ToList();
            playerLootChestLifeTime = Inputs.GetValue<float>("player_dead_chest_life_time", 0f);
            miningStationHP = Inputs.GetValue<float>("mining_station_hp", 0f);
        }

        public T GetValue<T>(string key)
        {
            if (this.Inputs.ContainsKey(key))
                return (T)this.Inputs[key];
            if(typeof(T) == typeof(float))
            {
                return (T)(object)(0.0f);
            }
            else if(typeof(T) == typeof(int))
            {
                return (T)(object)(0);
            }
            else if(typeof(T) == typeof(bool))
            {
                return (T)(object)(false);
            }
            else if(typeof(T) == typeof(float[]))
            {
                return (T)(object)(new float[] { 0.0f, 0.0f, 0.0f });
            }
            else if(typeof(T) == typeof(string[]))
            {
                return (T)(object)(new string[] { });
            }
            else if(typeof(T) == typeof(string))
            {
                return (T)(object)(string.Empty);
            }
            return default(T);
        }
    }
}
