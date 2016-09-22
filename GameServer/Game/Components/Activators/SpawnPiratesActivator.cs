using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;
using Space.Game.Drop;
using Space.Game.Inventory;
using Space.Game.Inventory.Objects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nebula.Game.Components.Activators {
    public class SpawnPiratesActivator : ActivatorObject {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private int mPirateCount;

        private List<NebulaObject> pirates = new List<NebulaObject>();
        private MmoWorld m_World;

        public void Init(SpawnPiratesActivatorComponentData data) {
            base.Init(data);
            mPirateCount = data.pirateCount;

        }

        public override void Start() {
            base.Start();
            m_World = nebulaObject.mmoWorld();
        }

        public override void OnActivate(NebulaObject source, out RPCErrorCode errorCode) {
            errorCode = RPCErrorCode.Ok;
            if(interactable) {
                if(pirates.Count == 0 ) {

                    if (existsPlayersInWorld) {
                        for (int i = 0; i < mPirateCount; i++) {
                            var obj = GeneratePirate();
                            //(nebulaObject.world as MmoWorld).AddObject(obj);
                            obj.AddToWorld();
                            pirates.Add(obj);
                        }
                        InteractOff();
                        log.InfoFormat("pirate activator started".Yellow());
                    } else {
                        errorCode = RPCErrorCode.NoPlayersAtZone;
                    }
                } else {
                    errorCode = RPCErrorCode.CollectionNotEmpty;
                }
            } else {
                errorCode = RPCErrorCode.ObjectNotInteractable;
            }
        }

        private bool existsPlayersInWorld {
            get {
                if(m_World.GetMmoActorsConcurrent((p)=> true).Count > 0 ) {
                    return true;
                }
                return false;
            }
        }

        private bool allIsDead {
            get {
                bool dead = true;
                foreach(var p in pirates ) {
                    if(p) {
                        dead = false;
                        break;
                    } 
                }
                return dead;
            }
        }

        private void CheckComplete() {
            if (pirates.Count > 0) {
                if (allIsDead) {
                    log.InfoFormat("Activator complete create chest".Yellow());
                    pirates.Clear();
                    SchemeDropper dropper = new SchemeDropper(CommonUtils.GetRandomEnumValue<Workshop>(new List<Workshop>()), 1, nebulaObject.resource, ObjectColor.orange);
                    SchemeObject scheme = dropper.Drop() as SchemeObject;
                    ConcurrentBag<ServerInventoryItem> dropList = new ConcurrentBag<ServerInventoryItem> { new ServerInventoryItem(scheme, 1) };
                    var obj = ObjectCreate.SharedChest((nebulaObject.world as MmoWorld), transform.position + Rand.UnitVector3() * 4, 5 * 60, dropList);
                    obj.AddToWorld();
                    SetCooldownTimer(cooldown);
                    SetInteractable(false);
                }
            }
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            CheckComplete();
            RPCErrorCode errorCode;
            OnActivate(null, out errorCode);
        }

       

        private GameObject GeneratePirate() {


            var startPosition = transform.position + Rand.UnitVector3() * radius * Rand.Float01();
            int subZone = (nebulaObject.world as MmoWorld).ResolvePositionSubzone(startPosition);

            Dictionary<ComponentID, ComponentData> components = new Dictionary<ComponentID, ComponentData>();
            components.Add(ComponentID.Skills, new SkillsComponentData());
            components.Add(ComponentID.Energy, new EnergyComponentData());
            components.Add(ComponentID.Raceable, new RaceableComponentData(Race.Humans));
            components.Add(ComponentID.PlayerAI, new PlayerAIStateComponentData());
            components.Add(ComponentID.CombatAI, new FreeFlyNearPointComponentData(true, 0.5f, 10, Server.AttackMovingType.AttackPurchase));
            components.Add(ComponentID.Ship, new BotShipComponentData(Difficulty.easy));
            components.Add(ComponentID.Weapon, new ShipWeaponComponentData(Difficulty.easy, 4));
            components.Add(ComponentID.Movable, new BotShipMovableComponentData());
            components.Add(ComponentID.Character, new BotCharacterComponentData(Workshop.DarthTribe, 1, FractionType.Pirate));
            components.Add(ComponentID.Damagable, new ShipDamagableComponentData(false));
            components.Add(ComponentID.Bonuses, new BonusesComponentData());
            components.Add(ComponentID.Target, new TargetComponentData());
            components.Add(ComponentID.NebulaObject, new NebulaObjectComponentData(ItemType.Bot, new Dictionary<byte, object>(), string.Empty, 1f, subZone));
            components.Add(ComponentID.Bot, new BotComponentData(BotItemSubType.StandardCombatNpc));

            NebulaObjectData data = new NebulaObjectData {
                ID = Guid.NewGuid().ToString(),
                position = startPosition,
                rotation = Vector3.Zero,
                componentCollection = components,
            };

            return ObjectCreate.NebObject(nebulaObject.world as MmoWorld, data);

            //ZoneNpcInfo npcInfo = new ZoneNpcInfo {
            //    aiType = new FreeFlyNearPointAIType { battleMovingType = AttackMovingType.AttackPurchase, radius = 10 },
            //    Difficulty = Difficulty.easy,
            //    fraction = FractionType.Pirate,
            //    Id = Guid.NewGuid().ToString(),
            //    level = 1,
            //    name = "XM",
            //    Position = (transform.position + Rand.UnitVector3() * radius * Rand.Float01()).ToArray(),
            //    Race = Race.Humans,
            //    RespawnInterval = -1,
            //    Rotation = new float[] { 0, 0, 0 },
            //    Workshop = Workshop.DarthTribe
            //};
            //return ObjectCreate.CombatNpc(nebulaObject.world as MmoWorld, npcInfo);

        }
    }
}
