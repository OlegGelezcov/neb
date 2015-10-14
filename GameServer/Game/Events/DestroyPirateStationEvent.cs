using GameMath;
using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Common;
using Nebula.Game.Components;
using Space.Game.Resources;
using Space.Game.Resources.Zones;
using Nebula.Game.Components.BotAI;
using Nebula.Engine;
using ExitGames.Logging;
using Nebula.Server;

namespace Nebula.Game.Events {
    public class DestroyPirateStationEvent : BaseEvent {

        private static ILogger log = LogManager.GetCurrentClassLogger();

        private string mPirateStationId;
        private List<string> mStationSecurityIds = new List<string>();
        private Vector3 mPirateStationPosition;
        private bool mStationDead;

        protected override bool CheckForComplete() {
            return mStationDead;
        }

        protected override void OnActivated() {
            mPirateStationId = string.Empty;
            mStationSecurityIds.Clear();
            mStationDead = false;

            CreatePirateStation();
            for(int i = 0; i < 5; i++) {
                CreateSecurityNpc();
            }
        }



        protected override void OnDiactivated() {
            
        }

        private void CreatePirateStation() {
            mPirateStationPosition = data.Position + Vector3.UnitY * 10f;

            Type[] stationComponents = new Type[] {
                typeof(PirateStationObject),
                typeof(EventedObject),
                typeof(NotShipDamagableObject),
                typeof(CharacterObject),
                typeof(RaceableObject),
                typeof(BotObject),
                typeof(ModelComponent),
                typeof(MmoMessageComponent)
            };

            int subZone = (nebulaObject.world as MmoWorld).ResolvePositionSubzone(mPirateStationPosition);

            GameObject pirateStation = new GameObject(mPirateStationPosition.ToVector(),
                new Hashtable(),
                Guid.NewGuid().ToString(),
                (byte)ItemType.Bot,
                nebulaObject.world as MmoWorld,
                new Dictionary<byte, object>(), 2f, 
                subZone,
                stationComponents);

            pirateStation.GetComponent<EventedObject>().SetOwnedEvent(this);
            pirateStation.GetComponent<NotShipDamagableObject>().SetMaximumHealth(5000);
            pirateStation.GetComponent<NotShipDamagableObject>().SetHealth(5000);
            pirateStation.GetComponent<CharacterObject>().SetFraction(FractionType.EventBot);
            pirateStation.GetComponent<CharacterObject>().SetLevel((nebulaObject.world as MmoWorld).Zone.Level);
            pirateStation.GetComponent<RaceableObject>().SetRace((byte)Race.Borguzands);
            pirateStation.GetComponent<CharacterObject>().SetWorkshop(CommonUtils.RandomWorkshop(Race.Borguzands));
            pirateStation.GetComponent<BotObject>().SetSubType(BotItemSubType.PirateStation);
            pirateStation.GetComponent<ModelComponent>().SetModelId("PS01");
            pirateStation.name = "Borguzand rogue station";
            mPirateStationId = pirateStation.Id;

            pirateStation.AddToWorld();
        }

        private void CreateSecurityNpc() {
            string npcTypeName = "station_security";
            //NpcTypeData npcTypeData;
            //if(!nebulaObject.world.Resource().NpcTypes.TryGetNpcType(npcTypeName, out npcTypeData)) {
            //    throw new Exception(string.Format("not founded npc data = {0}", npcTypeName));
            //}
            //FractionType fraction = (FractionType)Enum.Parse(typeof(FractionType), npcTypeData.Settings.Value<string>("fraction"));
            ZoneNpcInfo npcInfo = new ZoneNpcInfo {
                Difficulty = Difficulty.none,
                fraction = FractionType.EventBot,
                Id = Guid.NewGuid().ToString(),
                Position = (mPirateStationPosition + Rand.UnitVector3() * 30).ToArray(),
                Race = Race.Borguzands,
                RespawnInterval = -1,
                Rotation = new float[] { 0, 0, 0 },
                //TypeName = npcTypeName,
                Workshop = CommonUtils.RandomWorkshop(Race.Borguzands),
                level = (nebulaObject.world as MmoWorld).Zone.Level,
                name = "Station Security",
                aiType = new FreeFlyNearPointAIType { battleMovingType = AttackMovingType.AttackPurchase, radius = 30 }
            };

            Type[] components = new Type[] { CombatBaseAI.ResolveAI(npcInfo.aiType.movingType),
            typeof(ShipWeapon), typeof(BotShip), typeof(MmoMessageComponent), typeof(ShipBasedDamagableObject),
            typeof(PlayerBonuses), typeof(PlayerTarget), typeof(BotObject), typeof(PlayerSkills), typeof(ShipEnergyBlock),
            typeof(RaceableObject), typeof(AIState), typeof(CharacterObject), typeof(EventedObject), typeof(ShipMovable) };

            int subZone = (nebulaObject.world as MmoWorld).ResolvePositionSubzone(npcInfo.Position.ToVector3());

            GameObject obj = new GameObject(
                npcInfo.Position.ToVector(false),
                new Hashtable(),
                npcInfo.Id, (byte)ItemType.Bot,
                nebulaObject.world as MmoWorld,
                new Dictionary<byte, object> { { (byte)PS.Difficulty, (byte)npcInfo.Difficulty }, { (byte)PS.LightCooldown, 2.0f }, { (byte)PS.HeavyCooldown, 10.0f } }, 1f,
                subZone,
                components);
            var character = obj.GetComponent<CharacterObject>();
            character.SetWorkshop((byte)npcInfo.Workshop);
            character.SetLevel((nebulaObject.world as MmoWorld).Zone.Level);
            character.SetFraction(npcInfo.fraction);
            obj.GetComponent<BotShip>().GenerateModel();
            obj.GetComponent<ShipWeapon>().InitializeAsBot();
            obj.GetComponent<BotObject>().SetSubType((byte)BotItemSubType.StandardCombatNpc);
            obj.GetComponent<RaceableObject>().SetRace((byte)npcInfo.Race);
            obj.GetComponent<CombatBaseAI>().SetAIType(npcInfo.aiType);
            //obj.GetComponent<CombatBaseAI>().SetNpcTypeData(npcTypeData);
            obj.GetComponent<EventedObject>().SetOwnedEvent(this);
            obj.name = "Rogue";
            mStationSecurityIds.Add(obj.Id);
            obj.AddToWorld();
        }

        public void OnDeath(EventedObject obj) {
            log.Info("DestroyPirateStationEvent.OnDeath() called");
            if(active) {
                if(obj.nebulaObject.Id == mPirateStationId) {
                    mStationDead = true;
                }
            }
        }

        public void OnReceiveDamage(EventMessage message) {
            if(active) {
                if(message.Source.nebulaObject.Id == mPirateStationId || 
                    mStationSecurityIds.Contains(message.Source.nebulaObject.Id)) {
                    DamageInfo damageInfo = message.Data as DamageInfo;
                    if (damageInfo == null) {
                        return;
                    }
                    if (damageInfo.DamagerType != ItemType.Avatar) {
                        return;
                    }
                    NebulaObject damager;
                    nebulaObject.world.TryGetObject((byte)damageInfo.DamagerType, damageInfo.DamagerId, out damager);
                    if (damager) {
                        AddMember(damager);
                    }
                }
            }
        }
    }
}
