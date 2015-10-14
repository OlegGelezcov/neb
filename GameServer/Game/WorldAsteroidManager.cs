using Common;
using ExitGames.Logging;
using Nebula.Game.Components;
using Space.Game;
using Space.Game.Drop;
using Space.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game {
    public class WorldAsteroidManager {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly MmoWorld mWorld;
        private Dictionary<int, float> destroyTimes = new Dictionary<int, float>();
        public readonly AsteroidDropper asteroidDropper = new AsteroidDropper();

        public WorldAsteroidManager(MmoWorld world) {
            mWorld = world;
        }

        public void Update(float deltaTime) {
            var zoneAsteroidList = mWorld.Zone.Asteroids;
            if(zoneAsteroidList == null) {
                log.ErrorFormat("zone asteroid list is null at world {0}", mWorld.Name);
            }

            var existingAsteroidList = mWorld.ItemCache.GetItems((byte)ItemType.Asteroid)
                .Where(p => p.Value.GetComponent<AsteroidComponent>().zoneAsteroidInfo != null)
                .ToDictionary(p => p.Key, p => p.Value);

            
            foreach(var asteroidZoneInfo in zoneAsteroidList) {
                Item item = existingAsteroidList
                    .Values
                    .Where(it => it.GetComponent<AsteroidComponent>() && it.GetComponent<AsteroidComponent>().zoneAsteroidInfo.Index == asteroidZoneInfo.Index)
                    .FirstOrDefault();
                if(item) {
                    continue;
                }

                float destructionTime = DestructionTime(asteroidZoneInfo.Index);
                float interval = Time.curtime() - destructionTime;
                if(interval > asteroidZoneInfo.Respawn) {

                    bool dropSuccess = asteroidDropper.DropOccured(mWorld.Zone.Level,
                        mWorld.Resource().Asteroids.Data(asteroidZoneInfo.DataId),
                        asteroidZoneInfo.ForceCreate);
                    if(!dropSuccess) {
                        SetDestructionTime(asteroidZoneInfo.Index, Time.curtime());
                        continue;
                    }

                    var data = mWorld.Resource().Asteroids.Data(asteroidZoneInfo.DataId);
                    GameObject asteroidObj = ObjectCreate.Asteroid(mWorld, asteroidZoneInfo, data);
                    asteroidObj.AddToWorld(asteroidZoneInfo.Position, asteroidZoneInfo.Rotation);
                    
                   
                    //spawn asteroid
                }
            }
        }

        private float DestructionTime(int asteroidIndex) {
            if(!destroyTimes.ContainsKey(asteroidIndex)) {
                destroyTimes[asteroidIndex] = -1000000;
            }
            return destroyTimes[asteroidIndex];
        }

        public void SetDestructionTime(int index, float time) {
            destroyTimes[index] = time;
        }
    }
}
