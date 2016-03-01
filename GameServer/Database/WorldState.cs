using Common;
using ExitGames.Logging;
using MongoDB.Bson;
using Nebula.Game;
using Nebula.Server.Components;
using ServerClientCommon;
using Space.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class WorldState {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ObjectId Id { get; set; }
        public string worldID { get; set; }

        public List<string> destroyedObjects { get; set; }
        public Dictionary<string, NebulaObjectSave> saves { get; set; }

        public void Init(string inWorldID) {
            worldID = inWorldID;
            destroyedObjects = new List<string>();
            saves = new Dictionary<string, NebulaObjectSave>();
        }

        public void AddDestroyedObject(string databaseID) {
            if (!destroyedObjects.Contains(databaseID)) {
                destroyedObjects.Add(databaseID);
            }
        }

        public bool HasDestroyedObject(string databaseID) {
            CheckMembers();
            return destroyedObjects.Contains(databaseID);
        }

        //public void AddSave(string id, NebulaObjectSave save) {
        //    if(saves.ContainsKey(id)) {
        //        saves.Remove(id);
        //    }
        //    saves.Add(id, save);
        //}

        public void CheckMembers() {
            if(destroyedObjects == null) {
                destroyedObjects = new List<string>();
            }
            if(saves == null ) {
                saves = new Dictionary<string, NebulaObjectSave>();
            }
        }

        public void FillSaves(MmoWorld world) {
            CheckMembers();
            saves.Clear();

            var itemsToSave = world.GetItems((item) => item.databaseSaveable);
            foreach(var pItem in itemsToSave) {
                var save = NebulaObjectSave.FromGameObject(pItem.Value as GameObject);
                if(save != null ) {
                    if(saves.ContainsKey(save.id)) {
                        saves.Remove(save.id);
                    }
                    saves.Add(save.id, save);
                }
            }
        }

        public void RestoreObjectsFromSave(MmoWorld world) {
            CheckMembers();
            foreach(var pSave in saves) {
                var data = pSave.Value.ConvertToNebulaObjectData();
                if(data != null ) {

                    var nebObj = ObjectCreate.NebObject(world, data);
                    nebObj.AddToWorld();
                    nebObj.SetDatabaseSaveable(true);
                    log.InfoFormat("restored object from save = {0} [dy]", nebObj.Id);
                }
            }
        }
    }

    public class NebulaObjectSave {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public string id { get; set; }
        public float[] position { get; set; }
        public float[] rotation { get; set; }
        public Dictionary<int, Hashtable> components { get; set; }


        public NebulaObjectData ConvertToNebulaObjectData() {
            if(string.IsNullOrEmpty(id)) {
                log.InfoFormat("NebulaObjectSave: id is null [dy]");
                return null;
            }
            if(position == null ) {
                log.InfoFormat("NebulaObjectSave: position is null [dy]");
                return null;
            }
            if(rotation == null ) {
                log.InfoFormat("NebulaObjectSave: rotation is null [dy]");
                return null;
            }
            if(components == null ) {
                log.InfoFormat("NebulaObjectSave: components is null [dy]");
                return null;
            }

            Dictionary<ComponentID, ComponentData> componentCollection = new Dictionary<ComponentID, ComponentData>();
            foreach(var component in components) {
                var cData = GetComponentFromSave(component.Key, component.Value);
                if(cData != null) {
                    componentCollection.Add((ComponentID)component.Key, cData);
                }
            }

            return new NebulaObjectData {
                componentCollection = componentCollection,
                ID = id,
                position = position.ToVector3(),
                rotation = rotation.ToVector3()
            };
        }

        private ComponentData GetComponentFromSave(int componentID, Hashtable hash) {
            switch((ComponentID)componentID) {
                case ComponentID.NebulaObject:
                    return new NebulaObjectComponentData(hash);
                case ComponentID.Character:
                    return new BotCharacterComponentData(hash);
                case ComponentID.Raceable:
                    return new RaceableComponentData(hash);
                case ComponentID.Bonuses:
                    return new BonusesComponentData(hash);
                case ComponentID.Bot:
                    return new BotComponentData(hash);
                case ComponentID.MainOutpost:
                    return new MainOutpostComponentData(hash);
                case ComponentID.Model:
                    return new ModelComponentData(hash);
                case ComponentID.Damagable: {
                        if(hash.ContainsKey((int)SPC.SubType)) {
                            switch((ComponentSubType)(int)hash[(int)SPC.SubType]) {
                                case ComponentSubType.damagable_not_ship:
                                    return new NotShipDamagableComponentData(hash);
                                default:
                                    return new OutpostDamagableComponentData(hash);
                            }
                        } else {
                            return new OutpostDamagableComponentData(hash);
                        }
                    }
                    return new OutpostDamagableComponentData(hash);
                case ComponentID.Outpost:
                    return new OutpostComponentData(hash);
                case ComponentID.Turret:
                    return new TurretComponentData(hash);
                case ComponentID.Target:
                    return new TargetComponentData(hash);
                case ComponentID.Weapon:
                    return new SimpleWeaponComponentData(hash);
                case ComponentID.Movable:
                    return new SimpleMovableComponentData(hash);
                case ComponentID.CombatAI: {
                        if(hash.ContainsKey((int)SPC.SubType)) {
                            switch ((ComponentSubType)hash[(int)SPC.SubType]) {
                                case ComponentSubType.ai_wander_point:
                                    return new FreeFlyNearPointComponentData(hash);
                                case ComponentSubType.ai_stay:
                                    return new StayAIComponentData(hash);
                                default:
                                    return new FreeFlyNearPointComponentData(hash);
                            }
                        } else {
                            return new FreeFlyNearPointComponentData(hash);
                        }
                    }
                    
                case ComponentID.Teleport:
                    return new PersonalBeaconComponentData(hash);
                case ComponentID.FounderCube:
                    return new FounderCubeComponentData(hash);
                case ComponentID.MiningStation:
                    return new MiningStationComponentData(hash);
                default:
                    {
                        log.InfoFormat("Component from Save not defined for = {0} [dy]", (ComponentID)componentID);
                        return null;
                    }
            }
        }

        public static NebulaObjectSave FromGameObject(GameObject go) {
            if(go == null ) {
                return null;
            }
            if(!go.databaseSaveable) {
                return null;
            }

            Dictionary<int, Hashtable> savedComponents = new Dictionary<int, Hashtable>();

            foreach(object objID in go.componentIds) {
                var componentBehaviour = go.GetComponent((int)objID);
                if(componentBehaviour != null ) {
                    if(componentBehaviour is IDatabaseObject ) {
                        if(!savedComponents.ContainsKey((int)objID)) {
                            savedComponents.Add((int)objID, (componentBehaviour as IDatabaseObject).GetDatabaseSave());
                        }
                    }
                }
            }
            NebulaObjectComponentData nebulaObjectComponentData = new NebulaObjectComponentData((ItemType)go.Type, go.tags, go.badge, go.size, go.subZone);
            if(savedComponents.ContainsKey((int)ComponentID.NebulaObject) ) {
                savedComponents.Remove((int)ComponentID.NebulaObject);
            }
            savedComponents.Add((int)ComponentID.NebulaObject, go.GetDatabaseSave());

            return new NebulaObjectSave {
                id = go.Id,
                position = go.transform.position.ToArray(),
                rotation = go.transform.rotation.ToArray(),
                components = savedComponents
            };
        }
    }
}
