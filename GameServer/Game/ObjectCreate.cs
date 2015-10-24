using Common;
using GameMath;
using Nebula.Game.Components;
using Nebula.Game.Components.BotAI;
using Nebula.Game.Events;
using Space.Game;
using Space.Game.Resources;
using Space.Game.Resources.Zones;
using System;
using System.Collections;
using System.Collections.Generic;
using Nebula.Server;
using ExitGames.Logging;
using Nebula.Server.Components;
using Nebula.Game.Components.Activators;
using System.Collections.Concurrent;
using Space.Game.Inventory;

namespace Nebula.Game {
    public static class ObjectCreate {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static GameObject NebObject(MmoWorld world, NebulaObjectData nebulaObjectData) {
            List<Type> components = new List<Type>();
            NebulaObjectComponentData nebData = null;

            foreach(var comp in nebulaObjectData.componentCollection) {
                switch(comp.Key) {
                    case ComponentID.Model:
                        //components.Add(typeof(ModelComponent));
                        switch((comp.Value as MultiComponentData).subType) {
                            case ComponentSubType.simple_model:
                                components.Add(typeof(ModelComponent));
                                break;
                            case ComponentSubType.raceable_model:
                                components.Add(typeof(RaceableModelComponent));
                                break;
                        }
                        break;
                    case ComponentID.Movable:
                        switch((comp.Value as MultiComponentData).subType) {
                            case ComponentSubType.simple_movable:
                                components.Add(typeof(SimpleMovable));
                                break;
                            case ComponentSubType.bot_ship_movable:
                                components.Add(typeof(ShipMovable));
                                break;
                            case ComponentSubType.player_ship_movable:
                                components.Add(typeof(PlayerShipMovable));
                                break;
                        }
                        break;
                    case ComponentID.Activator:
                        {
                            switch((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.spawn_pirate_activator:
                                    components.Add(typeof(SpawnPiratesActivator));
                                    break;
                            }
                        }
                        break;
                    case ComponentID.Planet:
                        {
                            components.Add(typeof(PlanetObject));
                        }
                        break;
                    case ComponentID.CombatAI:
                        switch((comp.Value as MultiComponentData).subType) {
                            case ComponentSubType.ai_follow_path_combat:
                                components.Add(typeof(FollowPathCombatAI));
                                break;
                            case ComponentSubType.ai_follow_path_non_combat:
                                components.Add(typeof(FollowPathAI));
                                break;
                            case ComponentSubType.ai_orbit:
                                components.Add(typeof(OrbitCombatAI));
                                break;
                            case ComponentSubType.ai_patrol:
                                components.Add(typeof(PatrolBetweenAreasCombatAI));
                                break;
                            case ComponentSubType.ai_stay:
                                components.Add(typeof(StayCombatAI));
                                break;
                            case ComponentSubType.ai_wander_cube:
                                components.Add(typeof(WanderCombatAI));
                                break;
                            case ComponentSubType.ai_wander_point:
                                components.Add(typeof(WanderAroundPointCombatAI));
                                break;
                        }
                        break;
                    case ComponentID.Ship:
                        {
                            switch ((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.ship_bot:
                                    {
                                        components.Add(typeof(BotShip));
                                    }
                                    break;
                            }
                        }
                        break;
                    case ComponentID.Energy:
                        {
                            components.Add(typeof(ShipEnergyBlock));
                        }
                        break;
                    case ComponentID.DatabaseObject:
                        {
                            components.Add(typeof(DatabaseObject));
                        }
                        break;
                    case ComponentID.Bot:
                        {
                            components.Add(typeof(BotObject));
                        }
                        break;
                    case ComponentID.Raceable:
                        {
                            components.Add(typeof(RaceableObject));
                        }
                        break;
                    case ComponentID.Bonuses:
                        {
                            components.Add(typeof(PlayerBonuses));
                        }
                        break;
                    case ComponentID.MiningStation:
                        {
                            components.Add(typeof(MiningStation));
                        }
                        break;
                    case ComponentID.Respawnable:
                        {
                            components.Add(typeof(RespawnableObject));
                        }
                        break;
                    case ComponentID.Outpost:
                        {
                            components.Add(typeof(Outpost));
                        }
                        break;
                    case ComponentID.Target:
                        {
                            components.Add(typeof(PlayerTarget));
                        }
                        break;
                    case ComponentID.Asteroid:
                        {
                            components.Add(typeof(AsteroidComponent));
                        }
                        break;
                    case ComponentID.Skills:
                        {
                            components.Add(typeof(PlayerSkills));
                        }
                        break;
                    case ComponentID.PlayerAI:
                        {
                            components.Add(typeof(AIState));
                        }
                        break;
                    case ComponentID.Weapon:
                        {
                            switch ((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.weapon_ship:
                                    components.Add(typeof(ShipWeapon));
                                    break;
                                case ComponentSubType.weapon_simple:
                                    components.Add(typeof(SimpleWeapon));
                                    break;
                            }
                        }
                        break;
                    case ComponentID.Damagable:
                        {
                            switch((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.damagable_not_ship:
                                    components.Add(typeof(NotShipDamagableObject));
                                    break;
                                case ComponentSubType.damagable_ship:
                                    components.Add(typeof(ShipBasedDamagableObject));
                                    break;
                                case ComponentSubType.damagable_fixed_damage:
                                    components.Add(typeof(FixedInputDamageDamagableObject));
                                    break;
                                case ComponentSubType.damagable_outpost:
                                    components.Add(typeof(OutpostDamagable));
                                    break;
                            }
                        }
                        break;
                    case ComponentID.Character:
                        {
                            switch ((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.character_bot:
                                    components.Add(typeof(CharacterObject));
                                    break;
                                case ComponentSubType.character_player:
                                    components.Add(typeof(PlayerCharacterObject));
                                    break;
                            }
                        }
                        break;
                    case ComponentID.NebulaObject:
                        {
                            nebData = comp.Value as NebulaObjectComponentData;
                        }
                        break;
                    case ComponentID.Turret:
                        {
                            components.Add(typeof(Turret));
                        }
                        break;
                    case ComponentID.MainOutpost:
                        {
                            components.Add(typeof(MainOutpost));
                        }
                        break;
                    case ComponentID.SharedChest:
                        {
                            components.Add(typeof(SharedChest));
                        }
                        break;
                    case ComponentID.Teleport:
                        {
                            switch((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.SimpleTeleport:
                                    components.Add(typeof(Teleport));
                                    break;
                                case ComponentSubType.PersonalTeleport:
                                    components.Add(typeof(PersonalBeacon));
                                    break;
                            }
                            
                        }
                        break;
                    case ComponentID.SubZone:
                        {
                            components.Add(typeof(SubZoneComponent));
                        }
                        break;
                    case ComponentID.Station:
                        {
                            components.Add(typeof(StationComponent));
                        }
                        break;
                }
            }

            if(!components.Contains(typeof(MmoMessageComponent))) {
                components.Add(typeof(MmoMessageComponent));
            }

            Dictionary<byte, object> tags = new Dictionary<byte, object>();
            ItemType itemType = ItemType.Bot;
            float size = 1f;
            int subZoneID = 0;

            if(nebData != null ) {
                tags = nebData.tags;
                itemType = nebData.itemType;
                size = nebData.size;
                subZoneID = nebData.subZoneID;
            }

            GameObject nebObject = new GameObject(nebulaObjectData.position.ToVector(),
                new Hashtable(),
                nebulaObjectData.ID,
                (byte)itemType,
                world,
                tags,
                size,
                subZoneID,
                components.ToArray());

            if(nebData != null ) {
                nebObject.SetBadge(nebData.badge);
            }

            if (nebObject.GetComponent<CharacterObject>()) {
                if (nebulaObjectData.componentCollection.ContainsKey(ComponentID.Character)) {
                    nebObject.GetComponent<CharacterObject>().Init(nebulaObjectData.componentCollection[ComponentID.Character] as BotCharacterComponentData);
                }
            }

            foreach(var comp in nebulaObjectData.componentCollection) {
                switch(comp.Key) {
                    case ComponentID.Model:
                        //nebObject.GetComponent<ModelComponent>().Init(comp.Value as ModelComponentData);
                        {
                            switch((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.simple_model:
                                    nebObject.GetComponent<ModelComponent>().Init(comp.Value as ModelComponentData);
                                    break;
                                case ComponentSubType.raceable_model:
                                    nebObject.GetComponent<RaceableModelComponent>().Init(comp.Value as RaceableModelComponentData);
                                    break;
                            }
                        }
                        break;
                    case ComponentID.Movable:
                        {
                            switch((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.simple_movable:
                                    nebObject.GetComponent<SimpleMovable>().Init(comp.Value as SimpleMovableComponentData);
                                    break;
                                case ComponentSubType.bot_ship_movable:
                                    nebObject.GetComponent<ShipMovable>().Init(comp.Value as BotShipMovableComponentData);
                                    break;
                                case ComponentSubType.player_ship_movable:
                                    nebObject.GetComponent<PlayerShipMovable>().Init(comp.Value as PlayerShipMovableComponentData);
                                    break;
                            }
                        }
                        break;
                    case ComponentID.Activator:
                        {
                            switch((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.spawn_pirate_activator:
                                    nebObject.GetComponent<SpawnPiratesActivator>().Init(comp.Value as SpawnPiratesActivatorComponentData);
                                    break;
                            }
                        }
                        break;
                    case ComponentID.CombatAI:
                        {
                            switch((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.ai_follow_path_combat:
                                    nebObject.GetComponent<FollowPathCombatAI>().Init(comp.Value as FollowPathCombatAIComponentData);
                                    break;
                                case ComponentSubType.ai_follow_path_non_combat:
                                    nebObject.GetComponent<FollowPathAI>().Init(comp.Value as FollowPathNonCombatAIComponentData);
                                    break;
                                case ComponentSubType.ai_orbit:
                                    nebObject.GetComponent<OrbitCombatAI>().Init(comp.Value as OrbitAIComponentData);
                                    break;
                                case ComponentSubType.ai_patrol:
                                    nebObject.GetComponent<PatrolBetweenAreasCombatAI>().Init(comp.Value as PatrolAIComponentData);
                                    break;
                                case ComponentSubType.ai_stay:
                                    nebObject.GetComponent<StayCombatAI>().Init(comp.Value as StayAIComponentData);
                                    break;
                                case ComponentSubType.ai_wander_cube:
                                    nebObject.GetComponent<WanderCombatAI>().Init(comp.Value as WanderAIComponentData);
                                    break;
                                case ComponentSubType.ai_wander_point:
                                    nebObject.GetComponent<WanderAroundPointCombatAI>().Init(comp.Value as FreeFlyNearPointComponentData);
                                    break;
                            }
                        }
                        break;
                    case ComponentID.Ship:
                        {
                            switch ((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.ship_bot:
                                    nebObject.GetComponent<BotShip>().Init(comp.Value as BotShipComponentData);
                                    break;
                            }
                        }
                        break;
                    case ComponentID.Planet:
                        {
                            nebObject.GetComponent<PlanetObject>().Init(comp.Value as PlanetObjectComponentData);
                        }
                        break;
                    case ComponentID.Energy:
                        {
                            nebObject.GetComponent<ShipEnergyBlock>().Init(comp.Value as EnergyComponentData);
                        }
                        break;
                    case ComponentID.DatabaseObject:
                        {
                            nebObject.GetComponent<DatabaseObject>().Init(comp.Value as DatabaseObjectComponentData);
                        }
                        break;
                    case ComponentID.Bot:
                        {
                            nebObject.GetComponent<BotObject>().Init(comp.Value as BotComponentData);
                        }
                        break;
                    case ComponentID.Raceable:
                        {
                            nebObject.GetComponent<RaceableObject>().Init(comp.Value as RaceableComponentData);
                        }
                        break;
                    case ComponentID.Bonuses:
                        {
                            nebObject.GetComponent<PlayerBonuses>().Init(comp.Value as BonusesComponentData);
                        }
                        break;
                    case ComponentID.Respawnable:
                        {
                            nebObject.GetComponent<RespawnableObject>().Init(comp.Value as RespwanableComponentData);
                        }
                        break;
                    case ComponentID.Outpost:
                        {
                            nebObject.GetComponent<Outpost>().Init(comp.Value as OutpostComponentData);
                        }
                        break;
                    case ComponentID.Target:
                        {
                            nebObject.GetComponent<PlayerTarget>().Init(comp.Value as TargetComponentData);
                        }
                        break;
                    case ComponentID.MiningStation:
                        {
                            nebObject.GetComponent<MiningStation>().Init(comp.Value as MiningStationComponentData);
                        }
                        break;
                    case ComponentID.Weapon:
                        {
                            switch ((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.weapon_ship:
                                    nebObject.GetComponent<ShipWeapon>().Init(comp.Value as ShipWeaponComponentData);
                                    break;
                                case ComponentSubType.weapon_simple:
                                    nebObject.GetComponent<SimpleWeapon>().Init(comp.Value as SimpleWeaponComponentData);
                                    break;
                            }
                        }
                        break;
                    case ComponentID.Damagable:
                        {
                            switch((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.damagable_not_ship:
                                    nebObject.GetComponent<NotShipDamagableObject>().Init(comp.Value as NotShipDamagableComponentData);
                                    break;
                                case ComponentSubType.damagable_ship:
                                    nebObject.GetComponent<ShipBasedDamagableObject>().Init(comp.Value as ShipDamagableComponentData);
                                    break;
                                case ComponentSubType.damagable_fixed_damage:
                                    nebObject.GetComponent<FixedInputDamageDamagableObject>().Init(comp.Value as FixedInputDamageDamagableComponentData);
                                    break;
                                case ComponentSubType.damagable_outpost:
                                    nebObject.GetComponent<OutpostDamagable>().Init(comp.Value as OutpostDamagableComponentData);
                                    break;
                            }
                        }
                        break;
                    case ComponentID.Character:
                        {
                            switch ((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.character_bot:
                                    nebObject.GetComponent<CharacterObject>().Init(comp.Value as BotCharacterComponentData);
                                    break;
                                case ComponentSubType.character_player:
                                    nebObject.GetComponent<PlayerCharacterObject>().Init(comp.Value as PlayerCharacterComponentData);
                                    break;
                            }
                        }
                        break;
                    case ComponentID.Turret:
                        {
                            nebObject.GetComponent<Turret>().Init(comp.Value as TurretComponentData);
                        }
                        break;
                    case ComponentID.MainOutpost:
                        {
                            nebObject.GetComponent<MainOutpost>().Init(comp.Value as MainOutpostComponentData);
                        }
                        break;
                    case ComponentID.Asteroid:
                        {
                            nebObject.GetComponent<AsteroidComponent>().Init(comp.Value as AsteroidComponentData);
                        }
                        break;
                    case ComponentID.Skills:
                        {
                            nebObject.GetComponent<PlayerSkills>().Init(comp.Value as SkillsComponentData);
                        }
                        break;
                    case ComponentID.PlayerAI:
                        {
                            nebObject.GetComponent<AIState>().Init(comp.Value as PlayerAIStateComponentData);
                        }
                        break;
                    case ComponentID.SharedChest:
                        {
                            nebObject.GetComponent<SharedChest>().Init(comp.Value as SharedChestComponentData);
                        }
                        break;
                    case ComponentID.Teleport:
                        {
                            switch((comp.Value as MultiComponentData).subType) {
                                case ComponentSubType.SimpleTeleport:
                                    nebObject.GetComponent<Teleport>().Init(comp.Value as TeleportComponentData);
                                    break;
                                case ComponentSubType.PersonalTeleport:
                                    nebObject.GetComponent<PersonalBeacon>().Init(comp.Value as PersonalBeaconComponentData);
                                    break;
                            }
                        }
                        break;
                    case ComponentID.SubZone:
                        {
                            nebObject.GetComponent<SubZoneComponent>().Init(comp.Value as SubZoneComponentData);
                        }
                        break;
                    case ComponentID.Station:
                        {
                            nebObject.GetComponent<StationComponent>().Init(comp.Value as StationComponentData);
                        }
                        break;

                }
            }
            return nebObject;
        }

        public static GameObject Asteroid(MmoWorld world, ZoneAsteroidInfo asteroidZoneInfo, AsteroidData asteroidData) {
            int subZone = world.ResolvePositionSubzone(asteroidZoneInfo.Position.ToVector3());
            GameObject asteroidObject = new GameObject(
                asteroidZoneInfo.Position.ToVector(false),
                new Hashtable(),
                Guid.NewGuid().ToString(),
                (byte)ItemType.Asteroid,
                world,
                new Dictionary<byte, object>(),
                1f,
                subZone,
                new Type[] { typeof(AsteroidComponent), typeof(MmoMessageComponent), typeof(ModelComponent) }
                );
            asteroidObject.transform.SetRotation(asteroidZoneInfo.Rotation);
            asteroidObject.name = "Asteroid";
            AsteroidComponent asteroid = asteroidObject.GetComponent<AsteroidComponent>();
            if(string.IsNullOrEmpty(asteroidZoneInfo.model)) {
                log.Info("Asteroid without model");
            }
            asteroidObject.GetComponent<ModelComponent>().SetModelId(asteroidZoneInfo.model);

            asteroid.SetData(asteroidData);
            asteroid.SetZoneAsteroidInfo(asteroidZoneInfo);
            asteroid.Generate();
            return asteroidObject;
        }

        //public static GameObject Acticator(MmoWorld world, ActivatorObject.Init init, Vector3 position) {
        //    Type[] components = new Type[] { typeof(ActivatorObject), typeof(BotObject) };
        //    GameObject activatorObject = new GameObject(position.ToVector(),
        //        new Hashtable(),
        //        Guid.NewGuid().ToString(),
        //        (byte)ItemType.Bot,
        //        world,
        //        new Dictionary<byte, object>(),
        //        1f,
        //        components);
        //    activatorObject.name = "activator";
        //    activatorObject.GetComponent<ActivatorObject>().Initialize(init);
        //    return activatorObject;
        //}

        public static GameObject Chest(MmoWorld world, Vector3 position, float duration, ConcurrentDictionary<string, DamageInfo> inDamagers, ChestSourceInfo sourceInfo) {
            int subZone = world.ResolvePositionSubzone(position);

            GameObject chestObject = new GameObject(
                position.ToVector(),
                new Hashtable(),
                Guid.NewGuid().ToString(),
                (byte)ItemType.Chest,
                world,
                new Dictionary<byte, object>(),
                1f,
                subZone,
                new Type[] { typeof(ChestComponent) }
                );
            chestObject.name = "Container";
            ChestComponent chest = chestObject.GetComponent<ChestComponent>();
            chest.SetDuration(duration);
            chest.Fill(inDamagers, sourceInfo);
            return chestObject;
        }

        public static GameObject SharedChest(MmoWorld world, Vector3 position, float duration, ConcurrentBag<ServerInventoryItem> content ) {

            int subZone = world.ResolvePositionSubzone(position);

            GameObject chestObject = new GameObject(
                position.ToVector(),
                new Hashtable(),
                Guid.NewGuid().ToString(),
                (byte)ItemType.SharedChest,
                world,
                new Dictionary<byte, object>(),
                1f,
                subZone,
                new Type[] { typeof(SharedChest), typeof(ModelComponent) }
                );
            chestObject.name = "Container";
            var sharedChest = chestObject.GetComponent<SharedChest>();
            sharedChest.SetDuration(duration);
            foreach(var obj in content) {
                sharedChest.AddContent(obj);
            }
            chestObject.GetComponent<ModelComponent>().SetModelId("SHARED_CHEST");
            return chestObject;
        }


        public static GameObject CombatNpc(MmoWorld world, ZoneNpcInfo zoneNpcInfo) {

            Type botAi = null;



            botAi = CombatBaseAI.ResolveAI(zoneNpcInfo.aiType.movingType);

            Type[] components = new Type[] {
                botAi, typeof(ShipWeapon), typeof(BotShip),
                typeof(MmoMessageComponent), typeof(CharacterObject), typeof(ShipBasedDamagableObject),
                typeof(PlayerBonuses), typeof(PlayerTarget), typeof(BotObject), typeof(PlayerSkills), typeof(ShipEnergyBlock),
                typeof(RaceableObject), typeof(AIState), typeof(ShipMovable)
            };

            int subZone = 0;

            if(world != null)
                subZone = world.ResolvePositionSubzone(zoneNpcInfo.Position.ToVector3());

            GameObject obj = new GameObject(
                zoneNpcInfo.Position.ToVector(false), 
                new Hashtable(), 
                zoneNpcInfo.Id, 
                (byte)ItemType.Bot, 
                world, 
                new Dictionary<byte, object> {
                    { (byte)PS.Difficulty, (byte)zoneNpcInfo.Difficulty },
                    { (byte)PS.LightCooldown, 2.0f },
                    { (byte)PS.HeavyCooldown, 10.0f }
                }, 
                1f,
                subZone,
                components);

            var character = obj.GetComponent<CharacterObject>();
            character.SetWorkshop((byte)zoneNpcInfo.Workshop);
            if (world != null) {
                character.SetLevel((byte)world.Zone.Level);
            } else {
                character.SetLevel(1);
            }
            character.SetFraction(zoneNpcInfo.fraction);

            obj.GetComponent<BotShip>().GenerateModel();
            obj.GetComponent<ShipWeapon>().InitializeAsBot();
            obj.GetComponent<BotObject>().SetSubType((byte)BotItemSubType.StandardCombatNpc);
            obj.GetComponent<RaceableObject>().SetRace((byte)zoneNpcInfo.Race);
            obj.GetComponent<CombatBaseAI>().SetAIType(zoneNpcInfo.aiType);
            //obj.GetComponent<CombatBaseAI>().SetNpcTypeData(npcData);
            obj.name = "Test Bot";

            return obj;
        }

        public static GameObject EventNpc(MmoWorld world, BaseEvent evt, string npcTypeName) {
            switch(evt.data.Id) {
                case "E001":
                case "E002":
                case "E003":
                case "E004":
                    {
                        //NpcTypeData npcData;
                        //world.Resource().NpcTypes.TryGetNpcType(npcTypeName, out npcData);
                        //if(npcData == null ) { throw new Exception(string.Format("not founded npc data: {0}", npcTypeName)); }

                        //FractionType fraction = (FractionType)Enum.Parse(typeof(FractionType), npcData.Settings.Value<string>("fraction"));

                        ZoneNpcInfo npcInfo = new ZoneNpcInfo {
                            Difficulty = Difficulty.hard,
                            fraction = FractionType.EventBot,
                            Id = Guid.NewGuid().ToString(),
                            Position = evt.transform.position.ToArray(),
                            Race = world.Zone.InitiallyOwnedRace,
                            RespawnInterval = -1,
                            Rotation = new float[] { 0, 0, 0 },
                            Workshop = CommonUtils.RandomWorkshop(world.Zone.InitiallyOwnedRace),
                            level = world.Zone.Level,
                            name = "Raider",
                            aiType = new NoneAIType {  battleMovingType = AttackMovingType.AttackStay }
                        };

                        GameObject obj = CombatNpc(world, npcInfo);
                        obj.AddComponent<EventedObject>().SetOwnedEvent(evt);
                        return obj;
                    }
                default:
                    return null;
            }
            
        }
        
       public static GameObject Event(MmoWorld world, WorldEventData eventData) {
            Type[] components = null;
            switch(eventData.Id) {
                case "E001":
                case "E002":
                case "E003":
                case "E004":             
                    components = new Type[] { typeof(KillBossEvent) };
                    break;
                case "E005":
                    components = new Type[] { typeof(DestroyPirateStationEvent) };
                    break;
            
            }
            if(components == null) {
                throw new Exception(string.Format("event {0} don't has components", eventData.Id));
            }

            int subZone = world.ResolvePositionSubzone(eventData.Position);

            GameObject eventGameObject = new GameObject(
                eventData.Position.ToVector(),
                new Hashtable(),
                eventData.Id,
                (byte)ItemType.Event,
                world,
                new Dictionary<byte, object>(),
                1f,
                subZone,
                components);
            eventGameObject.GetComponent<BaseEvent>().SetEventData(eventData);

            return eventGameObject;
        }


    }

    
}
