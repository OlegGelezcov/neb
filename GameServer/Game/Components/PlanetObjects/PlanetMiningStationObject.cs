using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using ServerClientCommon;
using Nebula.Server.Nebula.Server.Components.PlanetObjects;
using System.Collections.Concurrent;
using Space.Game;
using Space.Game.Inventory;
using Space.Game.Inventory.Objects;
using GameMath;

namespace Nebula.Game.Components.PlanetObjects {
    public class PlanetMiningStationObject : PlanetObjectBase {

        private float m_InnerCounter = 0;
        
        private float m_CacheAcceleratorTimer = 0f;

        private RaceableObject m_RaceComponent;
        private bool m_HasAcceleratorFlag = false;

        private readonly ConcurrentDictionary<string, DamageInfo> m_Damagers = new ConcurrentDictionary<string, DamageInfo>();
        


        public override PlanetBasedObjectType objectType {
            get {
                return PlanetBasedObjectType.MiningStation;
            }
        }

        public override void Start() {
            base.Start();
            m_RaceComponent = GetComponent<RaceableObject>();
        }

        public override Hashtable GetBuildingProperties() {
            Hashtable hash = base.GetBuildingProperties();
            if(m_Data != null ) {
                var data = m_Data as MiningStationPlanetObjectComponentData;
                hash.Add((int)SPC.OreId, data.oreId);
                hash.Add((int)SPC.MaxCount, data.maxCount);
                hash.Add((int)SPC.CurrentCount, data.curCount);
                //hash.Add((int)SPC.Timer, data.lifeTimer);
                //hash.Add((int)SPC.Interval, data.life);
            }
            return hash;
        }

        private MiningStationPlanetObjectComponentData miningData {
            get {
                return m_Data as MiningStationPlanetObjectComponentData;
            }
        }

        private bool hasAccelerator {
            get {
                if (m_RaceComponent != null) {
                    var resourceAccelerator = nebulaObject.mmoWorld().Filter(obj => obj.GetComponent<PlanetResourceAcceleratorObject>() != null);

                    foreach (var acc in resourceAccelerator) {
                        var raceComponent = acc.GetInterface<RaceableObject>();
                        if (raceComponent != null) {
                            if(raceComponent.race == m_RaceComponent.race ) {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            var data = miningData;
            if(data != null ) {
                m_InnerCounter += deltaTime * data.speed * (m_HasAcceleratorFlag ? 2 : 1);
                if(m_InnerCounter > 1f ) {
                    int iCount = (int)m_InnerCounter;
                    m_InnerCounter -= ((int)m_InnerCounter);
                    data.AddCount(iCount);
                }



                m_CacheAcceleratorTimer += deltaTime;
                if(m_CacheAcceleratorTimer >= 10 ) {
                    m_HasAcceleratorFlag = hasAccelerator;
                    m_CacheAcceleratorTimer = 0;

                }
            }
        }

        public int count {
            get {
                var md = miningData;
                if(md != null ) {
                    return md.curCount;
                }
                return 0;
            }
        }

        public void ClearCount() {
            var md = miningData;
            if(md != null ) {
                md.ClearCount();
            }
            props.SetProperty((byte)PS.Info, GetBuildingProperties());
        }


        /// <summary>
        /// Called when this object was killed
        /// </summary>
        public void OnWasKilled() {
            var mData = miningData;
            if(mData == null || mData.curCount == 0) {
                return;
            }
            if(m_Damagers.Count > 0 ) {
                var entry = resource.planetOreMap.GetEntry(nebulaObject.mmoWorld().GetID());
                List<ServerInventoryItem> items = new List<ServerInventoryItem>();
                if (entry != null ) {
                    foreach(string oreId in entry.ores ) {
                        items.Add(new ServerInventoryItem(new MaterialObject(oreId), mData.curCount));
                    }
                }

                if(items.Count > 0 ) {
                    var chest = ObjectCreate.Chest(nebulaObject.mmoWorld(), transform.position + new Vector3(0, 1, 0) * 20.0f, 240, m_Damagers, items);
                    chest.AddToWorld();
                }
            }
        }

        /// <summary>
        /// Called when this object receive damage
        /// </summary>
        /// <param name="damager"></param>
        public override void OnNewDamage(DamageInfo damager) {
            base.OnNewDamage(damager);
            if(damager.isAvatar && (!m_Damagers.ContainsKey(damager.DamagerId))) {
                m_Damagers.TryAdd(damager.DamagerId, damager);
            }
        }

    }
}
