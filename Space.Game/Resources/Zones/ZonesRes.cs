using Common;
using Nebula.Server;
using Nebula.Server.Components;
using GameMath;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Space.Game.Resources.Zones {
    public class ZonesRes
    {
        public List<ZoneData> Zones { get; private set; }

        public void Load(string basePath) {
            string baseDirectory = Path.Combine(basePath, "Data/Zones");
            string[] files = Directory.GetFiles(baseDirectory, "*.xml", SearchOption.AllDirectories);
            Zones = new List<ZoneData>();
            foreach (string file in files) {
                Zones.AddRange(LoadFile(file));
            }
        }

        private List<ZoneData> LoadFile(string file )
        {
            XDocument document = XDocument.Load(file);
            var zoneList = document.Element("zones").Elements("zone").Select(e =>
            {
                var asteroids = e.Element("asteroids").Elements("asteroid").Select(ae =>
                {
                    float[] pos = ae.GetFloatArray("position");
                    float[] rot = ae.GetFloatArray("rotation");
                    float respawn = ae.GetFloat("respawn");
                    int index = int.Parse(ae.Attribute("index").Value);
                    string dataId = ae.Attribute("data_id").Value;

                    bool forceCreate = false;
                    if (ae.HasAttribute("force_create"))
                        forceCreate = bool.Parse(ae.Attribute("force_create").Value);

                    string model = ae.Attribute("model").Value;

                    return new ZoneAsteroidInfo { Index = index, Position = pos, Rotation = rot, Respawn = respawn, DataId = dataId, ForceCreate = forceCreate, model = model};

                }).ToList();

                List<ActivatorData> activators = new List<ActivatorData>();

                if (e.Element("activators") != null) {
                    activators = e.Element("activators").Elements("activator").Select(a => {
                        float[] pos = a.GetFloatArray("position");
                        string id = a.Attribute("id").Value;
                        float radius = a.GetFloat("radius");
                        int type = int.Parse(a.Attribute("type").Value);
                        string action = a.Attribute("action").Value;

                        return new ActivatorData {
                            Id = id,
                            Position = pos,
                            Radius = radius,
                            Type = type,
                            Action = action
                        };
                    }).ToList();
                }

                Race initiallyOwnedRace = (Race)(byte)int.Parse(e.Attribute("owned_race").Value);

                var events = e.Element("events").Elements("event").Select(ee => LoadEventData(ee)).ToList();

                Hashtable inputs = new Hashtable();
                e.Element("inputs").Elements("input").Select(ee =>
                    {
                        string key = ee.Attribute("key").Value;
                        object val = CommonUtils.ParseValue(ee.Attribute("value").Value, ee.Attribute("type").Value);
                        inputs.Add(key, val);
                        return key;
                    }).ToList();

                Dictionary<string, ZoneNpcInfo> npcs = LoadNpcs(e.Element("npcs"));
                List<string> npcGroups = this.LoadNpcGroups(e.Element("npc_groups"));
                List<ZonePlanetInfo> planets = LoadPlanets(e.Element("planets"));

                ConcurrentDictionary<string, NebulaObjectData> nebulaObjectCollection;
                if(e.Element("nebula_objects") != null ) {
                    nebulaObjectCollection = LoadNebulaObjects(e.Element("nebula_objects"));
                } else {
                    nebulaObjectCollection = new ConcurrentDictionary<string, NebulaObjectData>();
                }

                ZoneType zoneType = ZoneType.space;

                if(e.Attribute("zone_type") != null )
                {
                    zoneType = (ZoneType)Enum.Parse(typeof(ZoneType), e.Attribute("zone_type").Value);
                }

                WorldType worldType = WorldType.neutral;
                if(e.Attribute("world_type") != null ) {
                    worldType = (WorldType)Enum.Parse(typeof(WorldType), e.GetString("world_type"));
                }

                Vector3 hsp = new Vector3(0, 0, 0);
                Vector3 csp = new Vector3(0, 0, 0);
                Vector3 bsp = new Vector3(0, 0, 0);
                if(e.HasAttribute("h_sp")) {
                    hsp = e.GetFloatArray("h_sp").ToVector3();
                }
                if(e.HasAttribute("b_sp")) {
                    bsp = e.GetFloatArray("b_sp").ToVector3();
                }
                if(e.HasAttribute("c_sp")) {
                    csp = e.GetFloatArray("c_sp").ToVector3();
                }

                return new ZoneData
                {
                    Id = e.Attribute("id").Value,
                    Level = int.Parse(e.Attribute("level").Value),
                    Name = e.Attribute("name").Value,
                    Asteroids = asteroids,
                    InitiallyOwnedRace = initiallyOwnedRace,
                    Activators = activators,
                    Events = events,
                    Inputs = inputs,
                    Npcs = npcs,
                    NpcGroups = npcGroups,
                    Planets = planets,
                    ZoneType = zoneType,
                    nebulaObjects = nebulaObjectCollection,
                    worldType = worldType,
                    humanSP = hsp,
                     borguzandSP = bsp,
                      criptizidSP = csp
                };

            }).ToList();
            return zoneList;
        }

        private static ConcurrentDictionary<string, NebulaObjectData> LoadNebulaObjects(XElement parent) {
            ConcurrentDictionary<string, NebulaObjectData> result = new ConcurrentDictionary<string, NebulaObjectData>();
            var lst = parent.Elements("nebula_object").Select(no => {
                NebulaObjectData nebObjData = new NebulaObjectData {
                    ID = no.GetString("id"),
                    position = no.GetFloatArray("position").ToVector3(),
                    rotation = no.GetFloatArray("rotation").ToVector3(),
                };

                //if(no.HasAttribute("script")) {
                //    nebObjData.scriptFile = no.GetString("script");
                //}

                Dictionary<ComponentID, ComponentData> componentCollection = new Dictionary<ComponentID, ComponentData>();
                var lst2 = no.Elements("component").Select(ce => {
                    ComponentID componentID = (ComponentID)Enum.Parse(typeof(ComponentID), ce.GetString("id"));
                    switch(componentID) {
                        case ComponentID.Model:
                            {
                                ComponentSubType subType = ComponentSubType.simple_model;
                                if(ce.HasAttribute("sub_type")) {
                                    subType  = (ComponentSubType)Enum.Parse(typeof(ComponentSubType), ce.GetString("sub_type"));
                                }
                                switch(subType) {
                                    case ComponentSubType.simple_model:
                                        {
                                            ModelComponentData model = new ModelComponentData(ce);
                                            componentCollection.Add(componentID, model);
                                        }
                                        break;
                                    case ComponentSubType.raceable_model:
                                        {
                                            RaceableModelComponentData model = new RaceableModelComponentData(ce);
                                            componentCollection.Add(componentID, model);
                                        }
                                        break;
                                }
                                

                            }
                            break;
                        case ComponentID.Movable:
                            {
                                ComponentSubType subType = (ComponentSubType)Enum.Parse(typeof(ComponentSubType), ce.GetString("sub_type"));
                                switch(subType) {
                                    case ComponentSubType.simple_movable:
                                        SimpleMovableComponentData simpleMovableComponentData = new SimpleMovableComponentData(ce);
                                        componentCollection.Add(ComponentID.Movable, simpleMovableComponentData);
                                        break;
                                    case ComponentSubType.player_ship_movable:
                                        componentCollection.Add(ComponentID.Movable, new PlayerShipMovableComponentData());
                                        break;
                                    case ComponentSubType.bot_ship_movable:
                                        componentCollection.Add(ComponentID.Movable, new BotShipMovableComponentData());
                                        break;
                                }
                            }
                            break;
                        case ComponentID.Activator:
                            {
                                ComponentSubType subType = (ComponentSubType)Enum.Parse(typeof(ComponentSubType), ce.GetString("sub_type"));
                                switch(subType) {
                                    case ComponentSubType.spawn_pirate_activator:
                                        SpawnPiratesActivatorComponentData data = new SpawnPiratesActivatorComponentData(ce);
                                        componentCollection.Add(ComponentID.Activator, data);
                                        break;
                                }
                            }
                            break;
                        case ComponentID.CombatAI:
                            {
                                ComponentSubType subType = (ComponentSubType)Enum.Parse(typeof(ComponentSubType), ce.GetString("sub_type"));
                                switch(subType) {
                                    case ComponentSubType.ai_follow_path_combat:
                                        {
                                            FollowPathCombatAIComponentData data = new FollowPathCombatAIComponentData(ce);
                                            componentCollection.Add(ComponentID.CombatAI, data);
                                        }
                                        break;
                                    case ComponentSubType.ai_follow_path_non_combat:
                                        {
                                            FollowPathNonCombatAIComponentData data = new FollowPathNonCombatAIComponentData(ce);
                                            componentCollection.Add(ComponentID.CombatAI, data);
                                        }
                                        break;
                                    case ComponentSubType.ai_orbit:
                                        {
                                            OrbitAIComponentData data = new OrbitAIComponentData(ce);
                                            componentCollection.Add(ComponentID.CombatAI, data);
                                        }
                                        break;
                                    case ComponentSubType.ai_patrol:
                                        {
                                            PatrolAIComponentData data = new PatrolAIComponentData(ce);
                                            componentCollection.Add(ComponentID.CombatAI, data);
                                        }
                                        break;
                                    case ComponentSubType.ai_stay:
                                        {
                                            StayAIComponentData data = new StayAIComponentData(ce);
                                            componentCollection.Add(ComponentID.CombatAI, data);
                                        }
                                        break;
                                    case ComponentSubType.ai_wander_cube:
                                        {
                                            WanderAIComponentData data = new WanderAIComponentData(ce);
                                            componentCollection.Add(ComponentID.CombatAI, data);
                                        }
                                        break;
                                    case ComponentSubType.ai_wander_point:
                                        {
                                            FreeFlyNearPointComponentData data = new FreeFlyNearPointComponentData(ce);
                                            componentCollection.Add(ComponentID.CombatAI, data);
                                        }
                                        break;
                                }
                            }
                            break;
                        case ComponentID.DatabaseObject:
                            {
                                DatabaseObjectComponentData data = new DatabaseObjectComponentData(ce);
                                componentCollection.Add(ComponentID.DatabaseObject, data);
                            }
                            break;
                        case ComponentID.NebulaObject:
                            {
                                NebulaObjectComponentData data = new NebulaObjectComponentData(ce);
                                componentCollection.Add(ComponentID.NebulaObject, data);
                            }
                            break;
                        case ComponentID.Turret:
                            {
                                TurretComponentData data = new TurretComponentData(ce);
                                componentCollection.Add(ComponentID.Turret, data);
                            }
                            break;
                        case ComponentID.Raceable:
                            {
                                RaceableComponentData data = new RaceableComponentData(ce);
                                componentCollection.Add(ComponentID.Raceable, data);
                            }
                            break;
                        case ComponentID.Bonuses:
                            {
                                BonusesComponentData data = new BonusesComponentData(ce);
                                componentCollection.Add(ComponentID.Bonuses, data);
                            }
                            break;
                        case ComponentID.Respawnable:
                            {
                                RespwanableComponentData data = new RespwanableComponentData(ce);
                                componentCollection.Add(ComponentID.Respawnable, data);
                            }
                            break;
                        case ComponentID.Outpost:
                            {
                                OutpostComponentData data = new OutpostComponentData(ce);
                                componentCollection.Add(ComponentID.Outpost, data);
                            }
                            break;
                        case ComponentID.Target:
                            {
                                TargetComponentData data = new TargetComponentData(ce);
                                componentCollection.Add(ComponentID.Target, data);
                            }
                            break;
                        case ComponentID.Energy:
                            {
                                EnergyComponentData data = new EnergyComponentData(ce);
                                componentCollection.Add(ComponentID.Energy, data);
                            }
                            break;
                        case ComponentID.MiningStation:
                            {
                                MiningStationComponentData data = new MiningStationComponentData(ce);
                                componentCollection.Add(ComponentID.MiningStation, data);
                            }
                            break;
                        case ComponentID.Bot:
                            {
                                BotComponentData data = new BotComponentData(ce);
                                componentCollection.Add(ComponentID.Bot, data);
                            }
                            break;
                        case ComponentID.Asteroid:
                            {
                                AsteroidComponentData data = new AsteroidComponentData(ce);
                                componentCollection.Add(ComponentID.Asteroid, data);
                            }
                            break;
                        case ComponentID.Skills:
                            {
                                SkillsComponentData data = new SkillsComponentData(ce);
                                componentCollection.Add(ComponentID.Skills, data);
                            }
                            break;
                        case ComponentID.PlayerAI:
                            {
                                PlayerAIStateComponentData data = new PlayerAIStateComponentData(ce);
                                componentCollection.Add(ComponentID.PlayerAI, data);
                            }
                            break;
                        case ComponentID.Planet:
                            {
                                PlanetObjectComponentData data = new PlanetObjectComponentData(ce);
                                componentCollection.Add(ComponentID.Planet, data);
                            }
                            break;
                        case ComponentID.Ship:
                            {
                                ComponentSubType subType = (ComponentSubType)Enum.Parse(typeof(ComponentSubType), ce.GetString("sub_type"));
                                switch(subType) {
                                    case ComponentSubType.ship_bot:
                                        {
                                            BotShipComponentData data = new BotShipComponentData(ce);
                                            componentCollection.Add(ComponentID.Ship, data);
                                        }
                                        break;
                                }
                            }
                            break;
                        case ComponentID.Weapon:
                            {
                                ComponentSubType subType = (ComponentSubType)Enum.Parse(typeof(ComponentSubType), ce.GetString("sub_type"));
                                switch(subType) {
                                    case ComponentSubType.weapon_ship:
                                        {
                                            ShipWeaponComponentData data = new ShipWeaponComponentData(ce);
                                            componentCollection.Add(ComponentID.Weapon, data);
                                        }
                                        break;
                                    case ComponentSubType.weapon_simple:
                                        {
                                            SimpleWeaponComponentData data = new SimpleWeaponComponentData(ce);
                                            componentCollection.Add(ComponentID.Weapon, data);
                                        }
                                        break;
                                }
                            }
                            break;
                        case ComponentID.Damagable:
                            {
                                ComponentSubType subType = (ComponentSubType)Enum.Parse(typeof(ComponentSubType), ce.GetString("sub_type"));
                                switch(subType) {
                                    case ComponentSubType.damagable_not_ship:
                                        {
                                            NotShipDamagableComponentData data = new NotShipDamagableComponentData(ce);
                                            componentCollection.Add(ComponentID.Damagable, data);
                                        }
                                        break;
                                    case ComponentSubType.damagable_ship:
                                        {
                                            ShipDamagableComponentData data = new ShipDamagableComponentData(ce);
                                            componentCollection.Add(ComponentID.Damagable, data);
                                        }
                                        break;
                                    case ComponentSubType.damagable_fixed_damage:
                                        {
                                            FixedInputDamageDamagableComponentData data = new FixedInputDamageDamagableComponentData(ce);
                                            componentCollection.Add(ComponentID.Damagable, data);
                                        }
                                        break;
                                    case ComponentSubType.damagable_outpost:
                                        {
                                            OutpostDamagableComponentData data = new OutpostDamagableComponentData(ce);
                                            componentCollection.Add(ComponentID.Damagable, data);
                                        }
                                        break;
                                }
                            }
                            break;
                        case ComponentID.Character:
                            {
                                ComponentSubType subType = (ComponentSubType)Enum.Parse(typeof(ComponentSubType), ce.GetString("sub_type"));
                                switch(subType) {
                                    case ComponentSubType.character_bot:
                                        {
                                            BotCharacterComponentData data = new BotCharacterComponentData(ce);
                                            componentCollection.Add(ComponentID.Character, data);
                                        }
                                        break;
                                    case ComponentSubType.character_player:
                                        {
                                            PlayerCharacterComponentData data = new PlayerCharacterComponentData(ce);
                                            componentCollection.Add(ComponentID.Character, data);
                                        }
                                        break;
                                }
                            }
                            break;
                        case ComponentID.MainOutpost:
                            {
                                MainOutpostComponentData data = new MainOutpostComponentData(ce);
                                componentCollection.Add(ComponentID.MainOutpost, data);
                            }
                            break;
                        case ComponentID.SharedChest:
                            {
                                SharedChestComponentData data = new SharedChestComponentData(ce);
                                componentCollection.Add(ComponentID.SharedChest, data);
                            }
                            break;
                        case ComponentID.Teleport:
                            {
                                TeleportComponentData data = new TeleportComponentData(ce);
                                componentCollection.Add(ComponentID.Teleport, data);
                            }
                            break;
                        case ComponentID.SubZone:
                            {
                                SubZoneComponentData data = new SubZoneComponentData(ce);
                                componentCollection.Add(ComponentID.SubZone, data);
                            }
                            break;
                        case ComponentID.Station:
                            {
                                StationComponentData data = new StationComponentData(ce);
                                componentCollection.Add(ComponentID.Station, data);
                            }
                            break;
                    }
                    return componentID;
                }).ToList();
                nebObjData.componentCollection = componentCollection;
                result.TryAdd(nebObjData.ID, nebObjData);

                return nebObjData.ID;
            }).ToList();
            return result;
        }

        private List<string> LoadNpcGroups(XElement npcGroupsElement)
        {
            if(npcGroupsElement == null )
            {
                return new List<string>();
            }

            return npcGroupsElement.Elements("npc_group").Select(e => e.Attribute("id").Value).ToList();
        }

        private static WorldEventData LoadEventData(XElement eventElement)
        {
            string id = eventElement.Attribute("id").Value;
            Hashtable inputs = new Hashtable();
            var dumpList = eventElement.Element("inputs").Elements("input").Select(e =>
            {
                string key = e.Attribute("key").Value;
                string type = e.Attribute("type").Value;
                string valStr = e.Attribute("value").Value;
                inputs.Add(key, CommonUtils.ParseValue(valStr, type));
                return key;
            }).ToList();

            float cooldown = eventElement.GetFloat("cooldown");
            float radius = eventElement.GetFloat("radius");
            Vector3 position = eventElement.GetFloatArray("position").ToVector3();

            return new WorldEventData {
                Id = id,
                Inputs = inputs,
                Cooldown = cooldown,
                Position = position,
                Radius = radius
            };
        }

        private static List<ZonePlanetInfo> LoadPlanets(XElement parent)
        {
            List<ZonePlanetInfo> planets = new List<ZonePlanetInfo>();
            if (parent == null)
                return planets;

            planets = parent.Elements("planet").Select(planetElement =>
                {
                    string id = planetElement.Attribute("id").Value;
                    string name = planetElement.Attribute("name").Value;
                    Vector3 position = planetElement.Attribute("position").Value.ToVector().ToVector3();
                    int slotsForStation = planetElement.Attribute("slots_for_station").Value.ToInt();
                    string planetWorldId = planetElement.Attribute("planet_world_id").Value;
                    return new ZonePlanetInfo
                    {
                        Id = id,
                        Name = name,
                        PlanetWorldId = planetWorldId,
                        Position = position,
                        SlotsForStation = slotsForStation
                    };
                }).ToList();
            return planets;
        }
 

        private static Dictionary<string, ZoneNpcInfo> LoadNpcs(XElement parent )
        {
            return parent.Elements("npc").Select(e =>
            {
                string id = e.GetString("id");
                float[] position = e.GetFloatArray("position");
                float[] rotation = e.GetFloatArray("rotation");
                float respawnInterval = e.GetFloat("respawn_interval");
                Difficulty d = (Difficulty)Enum.Parse(typeof(Difficulty), e.Attribute("difficulty").Value);
                Race race = Race.None;
                if (e.HasAttribute("race"))
                    race = (Race)Enum.Parse(typeof(Race), e.Attribute("race").Value);
                FractionType fraction = FractionType.Pirate;
                if (e.HasAttribute("fraction"))
                    fraction = (FractionType)Enum.Parse(typeof(FractionType), e.GetString("fraction"));

                return new ZoneNpcInfo {
                    Id = id,
                    Position = position,
                    Rotation = rotation,
                    RespawnInterval = respawnInterval,
                    Workshop = e.GetEnum<Workshop>("workshop"),
                    Difficulty = d,
                    Race = race,
                    fraction = fraction,
                    name = e.GetString("name"),
                    level = e.GetInt("level"),
                    aiType = LoadAIType(e.Element("ai"))
                };
            }).ToDictionary(npc => npc.Id, npc => npc);
        }

        private static AIType LoadAIType(XElement element) {
            MovingType movingType = (MovingType)Enum.Parse(typeof(MovingType), element.GetString("name"));

            AttackMovingType attackMovingType = AttackMovingType.AttackIdle;
            if(element.HasAttribute("attack_moving_type")) {
                attackMovingType = (AttackMovingType)Enum.Parse(typeof(AttackMovingType), element.GetString("attack_moving_type"));
            }
            //bool chase = element.GetBool("chase");
            switch (movingType) {
                case MovingType.FreeFlyAtBox:
                    {
                        Vector3 min = element.GetFloatArray("min").ToVector3();
                        Vector3 max = element.GetFloatArray("max").ToVector3();
                        
                        return new FreeFlyAtBoxAIType {
                            corners = new MinMax { min = min, max = max },
                            battleMovingType = attackMovingType
                        };
                    }
                case MovingType.FreeFlyNearPoint:
                    {
                        float radius = element.GetFloat("radius");
                        return new FreeFlyNearPointAIType { battleMovingType = attackMovingType, radius = radius };
                    }
                case MovingType.OrbitAroundPoint:
                    {
                        float phiSpeed = element.GetFloat("phi_speed");
                        float thetaSpeed = element.GetFloat("theta_speed");
                        float radius = element.GetFloat("radius");
                        return new OrbitAroundPointAIType { battleMovingType = attackMovingType, phiSpeed = phiSpeed, thetaSpeed = thetaSpeed, radius = radius };
                    }
                case MovingType.Patrol:
                    {
                        Vector3 first = element.GetFloatArray("first").ToVector3();
                        Vector3 second = element.GetFloatArray("second").ToVector3();
                        return new PatrolAIType { battleMovingType = attackMovingType, firstPoint = first, secondPoint = second };
                    }
                case MovingType.None:
                    {
                        return new NoneAIType { battleMovingType = attackMovingType };
                    }
                case MovingType.FollowPathCombat:
                    {
                        Vector3[] path = element.ToVector3List("path").ToArray();
                        return new FollowPathAIType { battleMovingType = attackMovingType, path = path };

                    }
                case MovingType.FollowPathNonCombat:
                    {
                        Vector3[] path = element.ToVector3List("path").ToArray();
                        return new FollowPathNonCombatAIType { path = path };
                    }
                    break;
                default:
                    throw new Exception("undefined ai type: " + movingType);
            }
        }

        public bool ExistZone(string id)
        {
            var zone = this.Zones.Find(z => z.Id == id);
            return (zone != null) ? true : false;
        }

        public ZoneData Zone(string id)
        {
            return this.Zones.Find(z => z.Id == id);
        }

        public ZoneData Default(string id)
        {
            return new ZoneData {
                Id = id,
                Asteroids = new List<ZoneAsteroidInfo>(),
                Level = 1, Name = "(unknown)",
                Activators = new List<ActivatorData>(),
                Events = new List<WorldEventData>(),
                InitiallyOwnedRace = Race.None,
                Inputs = new Hashtable(),
                Npcs = new Dictionary<string, ZoneNpcInfo>(),
                NpcGroups = new List<string>(),
                borguzandSP = Vector3.Zero,
                criptizidSP = Vector3.Zero,
                humanSP = Vector3.Zero,
                nebulaObjects = new ConcurrentDictionary<string, NebulaObjectData>(),
                Planets = new List<ZonePlanetInfo>(),
                worldType = WorldType.neutral,
                ZoneType = ZoneType.space
            };
        }
    }
}
