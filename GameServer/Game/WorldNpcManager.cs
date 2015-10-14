using Common;
using Space.Game;
using Space.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game {
    public class WorldNpcManager {

        private MmoWorld mWorld;

        public WorldNpcManager(MmoWorld inWorld ) {
            mWorld = inWorld;
        }

        private Dictionary<string, float> mNpcDestroyTime = new Dictionary<string, float>();

        public void Update(float deltaTime) {
            foreach(var zoneNpcInfo in mWorld.Zone.Npcs) {
                if(zoneNpcInfo.Value.RespawnInterval <= 0f) {
                    continue;
                }

                Item item;
                if (mWorld.ItemCache.TryGetItem((byte)ItemType.Bot, zoneNpcInfo.Value.Id, out item)) {
                    continue;
                }

                if (DestroyTime(zoneNpcInfo.Value.Id) + zoneNpcInfo.Value.RespawnInterval >= Time.curtime()) {
                    continue;
                }

                ObjectCreate.CombatNpc(mWorld, zoneNpcInfo.Value).AddToWorld();

            }
        }

        private float DestroyTime(string id) {
            if(mNpcDestroyTime.ContainsKey(id)) {
                return mNpcDestroyTime[id];
            }
            mNpcDestroyTime.Add(id, -1000000f);
            return mNpcDestroyTime[id];
        }

        public void SetDestroyTime(string id, float time) {
            if (mWorld.Zone.Npcs.ContainsKey(id)) {
                mNpcDestroyTime[id] = time;
            }
        }
    }
}
