using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Nebula.Server.Components;

namespace Nebula.Server.Nebula.Server.Components.PlanetObjects {
    public class MiningStationPlanetObjectComponentData : PlanetObjectBaseComponentData {
        public string oreId { get; private set; }
        public int maxCount { get; private set; }
        public int curCount { get; private set; }
        //public float lifeTimer { get; private set; }
        //public float lifeInterval { get; private set; }
        public float speed { get; private set; }

        public MiningStationPlanetObjectComponentData(XElement element)
            : base(element) {
            oreId = element.GetString("ore_id");
            maxCount = element.GetInt("max_count");
            curCount = element.GetInt("cur_count");
            //lifeTimer = element.GetFloat("life_timer");
            //lifeInterval = element.GetFloat("life_interval");
            speed = element.GetFloat("speed");
        }

        public MiningStationPlanetObjectComponentData(Hashtable hash )
            : base(hash) {
            oreId = hash.GetValue<string>((int)SPC.OreId, string.Empty);
            maxCount = hash.GetValue<int>((int)SPC.MaxCount, 0);
            curCount = hash.GetValue<int>((int)SPC.CurrentCount, 0);
            //lifeTimer = hash.GetValue<float>((int)SPC.Timer, 0f);
            //lifeInterval = hash.GetValue<float>((int)SPC.Interval, 0f);
            speed = hash.GetValue<float>((int)SPC.Speed, 0f);
        }

        public MiningStationPlanetObjectComponentData(int row, int column, PlanetBasedObjectType objectType, string ownerId,
            string oreId, int maxCount, int curCount, float lifeTimer, float lifeInterval, float speed, string characterId, string characterName, string coalitionName)
            : base(row, column, objectType, ownerId, lifeInterval, lifeTimer, characterId, characterName, coalitionName ) {
            this.oreId = oreId;
            this.maxCount = maxCount;
            this.curCount = curCount;
            //this.lifeTimer = lifeTimer;
            //this.lifeInterval = lifeInterval;
            this.speed = speed;
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.planet_object_mining_station;
            }
        }

        public override Hashtable AsHash() {
            Hashtable hash =  base.AsHash();
            hash.Add((int)SPC.OreId, oreId);
            hash.Add((int)SPC.MaxCount, maxCount);
            hash.Add((int)SPC.CurrentCount, curCount);
            //hash.Add((int)SPC.Timer, lifeTimer);
            //hash.Add((int)SPC.Interval, lifeInterval);
            hash.Add((int)SPC.Speed, speed);
            return hash;
        }

        public void AddCount(int cnt) {
            curCount += cnt;
            curCount = (int)GameMath.Mathf.Clamp(curCount, 0, maxCount);
        }

        public void ClearCount() {
            curCount = 0;
        }
    }
}
