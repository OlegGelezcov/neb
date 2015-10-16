using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Server.Components;
using Space.Game;
using System.Collections.Concurrent;

namespace Nebula.Game {
    public class MmoWorldNebulaObjectManager {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly MmoWorld mWorld;
        private ConcurrentDictionary<string, RespawnData> mRespawns = new ConcurrentDictionary<string, RespawnData>();
        private readonly ConcurrentDictionary<string, NebulaObjectData> mNebulaObjects;
        private bool mInitiallyCreated = false;

        //private SpawnCheckManager mSpawnCheckManager = new SpawnCheckManager();


        public MmoWorldNebulaObjectManager(MmoWorld world) {
            mWorld = world;
            mNebulaObjects = mWorld.Zone.nebulaObjects;

            //foreach(var nebObj in mNebulaObjects) {
            //    if(!string.IsNullOrEmpty(nebObj.Value.scriptFile)) {
            //        nebObj.Value.script.Load(mWorld, System.IO.Path.Combine(mWorld.Resource().path, "Data/ServerScripts/" + nebObj.Value.scriptFile + ".txt"));
            //    }
            //}
        }

        private bool CreateObjectsOnStart() {
            if (!mInitiallyCreated) {
                mInitiallyCreated = true;
                foreach (var p in mNebulaObjects) {
                    TryCreate(p.Value);
                }
                return true;
            }
            return false;
        }

        private NebulaObject TryCreate(NebulaObjectData data) {
            bool allowCreate = true;
            string databaseID = data.databaseID;
            if (!string.IsNullOrEmpty(databaseID) && mWorld.HasDestroyedObject(databaseID)) {
                allowCreate = false;
            }
            if (allowCreate) {
                var obj = ObjectCreate.NebObject(mWorld, data);
                obj.AddToWorld();
                return obj;
            } else {
                log.InfoFormat("object = {0} was destroyed and saved, don't create [blue]", data.ID);
                return null;
            }
        }



        public void Update(float deltaTime ) {

            if (CreateObjectsOnStart()) {
                return;
            }

            float currentTime = Time.curtime();
            foreach(var pRespawnData in mRespawns) {
                var respawnData = pRespawnData.Value;
                if(respawnData.handled) {
                    continue;
                }

                if(currentTime > respawnData.time) {
                    if(mWorld.HasItem(respawnData.ID, respawnData.Type)) {
                        respawnData.handled = true;
                        continue;
                    }

                    NebulaObjectData createData;
                    if(mNebulaObjects.TryGetValue(respawnData.ID, out createData)) {
                        var obj = TryCreate(createData);
                        //var obj = ObjectCreate.NebObject(mWorld, createData);
                        //obj.AddToWorld();
                        if (obj != null) {
                            LogObject(obj);
                        }
                    }
                    respawnData.handled = true;
                }
            }
            DeleteHandledRespwans();

        }


        private void LogObject(NebulaObject obj) {
            var bot = obj.GetComponent<BotObject>();
            if (bot != null) {
                BotItemSubType subType = (BotItemSubType)bot.botSubType;
                if (subType == BotItemSubType.Turret) {
                    log.InfoFormat("turret in world = {0} respawned [purple]", mWorld.Name);
                } else if (subType == BotItemSubType.Outpost) {
                    log.InfoFormat("fortification in world = {0} respawned [purple]", mWorld.Name);
                } else if (subType == BotItemSubType.MainOutpost) {
                    log.InfoFormat("outpost in world = {0} respawned [purple]", mWorld.Name);
                }
            }
        }

        private void DeleteHandledRespwans() {
            ConcurrentBag<string> ids = new ConcurrentBag<string>();
            foreach(var p in mRespawns) {
                if(p.Value.handled) {
                    ids.Add(p.Key);
                }
            }

            foreach(string id in ids ) {
                RespawnData r;
                mRespawns.TryRemove(id, out r);
            }
        }


        public void SetRespawnData(RespawnData data) {
            //log.InfoFormat("set respawn data for id = {0}", data.ID);
            if(mRespawns.ContainsKey(data.ID)) {
                RespawnData r;
                if(mRespawns.TryRemove(data.ID, out r)) {
                    mRespawns.TryAdd(data.ID, data);
                }
            } else {
                mRespawns.TryAdd(data.ID, data);
            }
        }

    }
}
