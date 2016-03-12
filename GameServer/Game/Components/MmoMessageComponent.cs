// MmoMessageComponent.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 7:02:27 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Nebula.Game.Components {
    using Common;
    using Contracts;
    using ExitGames.Logging;
    using GameMath;
    //using global::Common;
    using Nebula.Engine;
    using Pets;
    using Photon.SocketServer;
    using ServerClientCommon;
    using Space.Game;
    using Space.Server;
    using Space.Server.Events;
    using Space.Server.Messages;
    using System.Collections;
    using System.Linq;

    public class MmoMessageComponent : NebulaBehaviour {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public override void Start() {
            
        }

        private int skillUseIndex = 1;


        public void PublishSkillUsed(EventReceiver receiver, NebulaObject target, PlayerSkill skill, bool success, string message, Hashtable result) {
            if(skill.IsEmpty) {
                return;
            }
            if(!(nebulaObject)) {
                return;
            }

            if(false == nebulaObject.IsPlayer() ) {
                if(receiver == EventReceiver.OwnerAndSubscriber) {
                    receiver = EventReceiver.ItemSubscriber;
                } else if(receiver == EventReceiver.ItemOwner) {
                    return;
                }
            }

            skillUseIndex++;
            if(skillUseIndex == int.MaxValue) {
                skillUseIndex = 1;
            }
            Hashtable data = new Hashtable {
                {(int)SPC.IsOn, skill.isOn },
                {(int)SPC.Data, skill.data.GetInfo() },
                {(int)SPC.Source, nebulaObject.Id },
                {(int)SPC.SourceType, nebulaObject.Type },
                {(int)SPC.Target, target.Id },
                {(int)SPC.TargetType, target.Type },
                {(int)SPC.IsSuccess, success },
                {(int)SPC.Message, message },
                {(int)SPC.Info, result},
                {(int)SPC.Index, skillUseIndex }
            };
            SendParameters sendParameters = new SendParameters { Unreliable = false, ChannelId = Settings.ItemEventChannel };
            UseSkillEvent evt = new UseSkillEvent { Properties = data };
            EventData evtData = new EventData((byte)EventCode.UseSkill, evt);
            var msg = new ItemEventMessage(nebulaObject as Item, evtData, sendParameters);

            switch(receiver) {
                case EventReceiver.ItemOwner:
                    ReceiveEvent(evtData, sendParameters);
                    break;
                case EventReceiver.ItemSubscriber:
                    (nebulaObject as Item).EventChannel.Publish(msg);
                    break;
                case EventReceiver.OwnerAndSubscriber:
                    ReceiveEvent(evtData, sendParameters);
                    (nebulaObject as Item).EventChannel.Publish(msg);
                    break;
            }

        }

        public void ReceiveServiceMessage(ServiceMessageType messageType, string message) {
            if(nebulaObject) {
                var eventInstance = new ItemGeneric {
                    ItemId = nebulaObject.Id,
                    ItemType = nebulaObject.Type,
                    CustomEventCode = (byte)CustomEventCode.ServiceMessage,
                    EventData = new Hashtable {
                        {(int)SPC.Type, (byte)messageType },
                        {(int)SPC.Message, message }
                    }
                };
                var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
                SendParameters sendParameters = new SendParameters {
                    Unreliable = false,
                    ChannelId = Settings.ItemEventChannel
                };
                ReceiveEvent(eventData, sendParameters);
            }
        }


        public void ReceiveMiningStation(Hashtable info) {
            var evt = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.GetMiningStationInfo,
                EventData = info
            };
            var evtData = new EventData((byte)EventCode.ItemGeneric, evt);
            SendParameters sendParameters = new SendParameters {
                Unreliable = false,
                ChannelId = Settings.ItemEventChannel
            };
            ReceiveEvent(evtData, sendParameters);
        }

        public void ReceivePlanetInfo(Hashtable info) {
            var evt = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.GetPlanetInfo,
                EventData = info
            };
            var evtData = new EventData((byte)EventCode.ItemGeneric, evt);
            SendParameters sendParameters = new SendParameters {
                Unreliable = false,
                ChannelId = Settings.ItemEventChannel
            };
            ReceiveEvent(evtData, sendParameters);
        }

        public void ReceivePetsUpdate() {
            var petManager = GetComponent<PetManager>();
            if(petManager != null ) {
                var evtInstance = new ItemGeneric {
                    ItemId = nebulaObject.Id,
                    ItemType = nebulaObject.Type,
                    CustomEventCode = (byte)CustomEventCode.PetsUpdate,
                    EventData = petManager.GetInfo()
                };
                var eventData = new EventData((byte)EventCode.ItemGeneric, evtInstance);
                SendParameters sendParameters = new SendParameters {
                    Unreliable = false,
                    ChannelId = Settings.ItemEventChannel
                };
                ReceiveEvent(eventData, sendParameters);
            }
        }
        public void ReceivePassiveBonusComplete(Hashtable info) {
            var evt = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.PAssiveBonusCompleted,
                EventData = info
            };
            var evtData = new EventData((byte)EventCode.ItemGeneric, evt);
            SendParameters sendParameters = new SendParameters {
                Unreliable = false,
                ChannelId = Settings.ItemEventChannel
            };
            ReceiveEvent(evtData, sendParameters);
        }

        public void ReceivePassiveBonuses(Hashtable info) {
            var evt = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.PassiveBonusesUpdate,
                EventData = info
            };
            var evtData = new EventData((byte)EventCode.ItemGeneric, evt);
            SendParameters sendParameters = new SendParameters {
                Unreliable = false,
                ChannelId = Settings.ItemEventChannel
            };
            ReceiveEvent(evtData, sendParameters);
        }

        public void SendDamageDron(Hashtable info) {
            var evt = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.DamageDron,
                EventData = info
            };
            var evtData = new EventData((byte)EventCode.ItemGeneric, evt);
            SendParameters sendParameters = new SendParameters {
                Unreliable = false,
                ChannelId = Settings.ItemEventChannel
            };
            SendEventData(EventReceiver.OwnerAndSubscriber, evtData, sendParameters);
        }

        public void SendHealDron(Hashtable info) {
            var evt = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.HealDron,
                EventData = info
            };
            var evtData = new EventData((byte)EventCode.ItemGeneric, evt);
            SendParameters sendParameters = new SendParameters {
                Unreliable = false,
                ChannelId = Settings.ItemEventChannel
            };
            SendEventData(EventReceiver.OwnerAndSubscriber, evtData, sendParameters);
        }


        public void ReceiveUpdateCombatStats() {
            var evt = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.UpdateCombatStats,
                EventData = GetComponent<MmoActor>().ActionExecutor.GetCombatParams()
            };
            var evtData = new EventData((byte)EventCode.ItemGeneric, evt);
            SendParameters sendParameters = new SendParameters { Unreliable = true, ChannelId = Settings.ItemEventChannel };
            ReceiveEvent(evtData, sendParameters);
        }

        public void ReceiveTeleportJump(Hashtable status) {
            var evt = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.TeleportJump,
                EventData = status
            };
            var evtData = new EventData((byte)EventCode.ItemGeneric, evt);
            SendParameters sendParameters = new SendParameters { Unreliable = true, ChannelId = Settings.ItemEventChannel };
            ReceiveEvent(evtData, sendParameters);
        }

        public void ReceiveExp(int exp) {
            var evt = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.ReceiveExp,
                EventData = new Hashtable { { (int)SPC.Exp, exp }  }
            };
            var evtData = new EventData((byte)EventCode.ItemGeneric, evt);
            SendParameters sendParameters = new SendParameters { Unreliable = true, ChannelId = Settings.ItemEventChannel };
            ReceiveEvent(evtData, sendParameters);
        }

        /// <summary>
        /// Call only on players, send event to client change position on client, if item not player, nothing sended
        /// </summary>
        /// <param name="pos">Position to will be setted</param>
        public void SetPos(Vector3 pos) {

            if(false == nebulaObject.IsPlayer()) {
                return;
            }

            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.SetPos,
                EventData = new Hashtable {
                    { (int)SPC.Position, pos.ToArray() }
                }
            };

            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendParameters sendParameters = new SendParameters {
                Unreliable = false,
                ChannelId = Settings.ItemEventChannel
            };
            ReceiveEvent(eventData, sendParameters);
        }
       

        public void StartJumpToPosition(Vector3 pos, int skillID = -1) {
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.JumpToPosition,
                EventData = new Hashtable {
                    { (int)SPC.Position, pos.ToArray() },
                    { (int)SPC.Skill, skillID }
                }
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendParameters sendParameters = new SendParameters {
                Unreliable = false,
                ChannelId = Settings.ItemEventChannel
            };

            if (skillID == -1) {
                ReceiveEvent(eventData, sendParameters);
            } else {
                if (nebulaObject.IsPlayer()) {
                    SendEventData(EventReceiver.OwnerAndSubscriber, eventData, sendParameters);
                } else {
                    SendEventData(EventReceiver.ItemSubscriber, eventData, sendParameters);
                }
            }
        }

        public void ReceiveGameEvent(string gameEventId, bool active) {
            if(nebulaObject) {
                var eventInstance = new ItemGeneric {
                    ItemId = nebulaObject.Id, 
                    ItemType = nebulaObject.Type,
                    CustomEventCode = (byte)CustomEventCode.GameEvent,
                    EventData = new Hashtable { { (int)SPC.Id, gameEventId }, { (int)SPC.Active, active} }
                };
                SendParameters sendParameters = new SendParameters { ChannelId = Settings.ItemEventChannel, Unreliable = true };
                var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
                ReceiveEvent(eventData, sendParameters);
            }
        }

        public void ReceiveWorldRaceChanged(Hashtable info) {
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.WorldRaceChanged,
                EventData = info
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            EventData eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            ReceiveEvent(eventData, sendParameters);
        }


        public void SendActivatorEvent(bool active) {
            if(!nebulaObject) {
                return;
            }

            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.Activator,
                EventData = new Hashtable { { (int)SPC.Active, active } }
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);

            var msg = new ItemEventMessage(nebulaObject as Item, eventData, sendParameters);
            (nebulaObject as Item).EventChannel.Publish(msg);
        }

        public void SendCollectChest(Hashtable hash) {
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.CollectChest,
                EventData = hash
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendEventData(EventReceiver.ItemSubscriber, eventData, sendParameters);
        }

        public void SendResurrect() {
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.Resurrect,
                EventData = new Hashtable()
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            if(nebulaObject.IsPlayer()) {
                SendEventData(EventReceiver.OwnerAndSubscriber, eventData, sendParameters);
            } else {
                SendEventData(EventReceiver.ItemSubscriber, eventData, sendParameters);
            }
        }

        public void SendSubZoneUpdate(EventReceiver receiver, int subZone) {
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.UpdateSubZone,
                EventData = subZone
            };
            SendParameters sendParameters = new SendParameters { ChannelId = Settings.ItemEventChannel, Unreliable = false };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendEventData(receiver, eventData, sendParameters);
        }

        public void SendKilled(EventReceiver receiver) {
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.WasKilled,
                EventData = new Hashtable()
            };
            SendParameters sendParameters = new SendParameters { ChannelId = Settings.ItemEventChannel, Unreliable = false };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendEventData(receiver, eventData, sendParameters);
        }

        public void ResurrectBySkillEffect() {
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.ResurrectByKillEffect,
                EventData = new Hashtable()
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendEventData(EventReceiver.OwnerAndSubscriber, eventData, sendParameters);
        }

        
        private void SendEventData(EventReceiver receiver, EventData eventData, SendParameters sendParameters) {
            switch (receiver) {
                case EventReceiver.ItemOwner:
                    ReceiveEvent(eventData, sendParameters);
                    break;
                case EventReceiver.ItemSubscriber:
                    {
                        var msg = new ItemEventMessage(nebulaObject as Item, eventData, sendParameters);
                        (nebulaObject as Item).EventChannel.Publish(msg);
                    }
                    break;
                case EventReceiver.OwnerAndSubscriber:
                    {
                        ReceiveEvent(eventData, sendParameters);
                        var msg = new ItemEventMessage(nebulaObject as Item, eventData, sendParameters);
                        (nebulaObject as Item).EventChannel.Publish(msg);
                    }
                    break;

            }
        }

        private int shotID = 1;
        //private int nextShotID() {
        //    Interlocked.Increment(ref shotID);
        //    if(shotID == int.MaxValue) {
        //        Interlocked.Exchange(ref shotID, 1);
        //    }
        //    return shotID;
        //}

        public void SendPetSkillUsed(Hashtable properties) {
            if(!nebulaObject) { return; }
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.PetSkillUsed,
                EventData = properties
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendEventData(EventReceiver.ItemSubscriber, eventData, sendParameters);
        }

        public void SendShot(EventReceiver receiver, Hashtable shotProperties) {
            if(!nebulaObject) {
                return;
            }
            if(!nebulaObject.IsPlayer() && (receiver != EventReceiver.ItemSubscriber)) {
                receiver = EventReceiver.ItemSubscriber;
            }

            //log.InfoFormat("shot from {0}=>{1}", (ItemType)nebulaObject.Type, (ItemType)(byte)shotProperties[(int)SPC.TargetType]);

            shotProperties.Add((int)SPC.ShotID, shotID);
            shotID++;
            if(shotID == int.MaxValue) {
                shotID = 1;
            }

            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.Fire,
                EventData = shotProperties
            };

            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };

            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);

            //try fix double event ( this don't work after moving in new world)
            /*
            if (receiver == EventReceiver.OwnerAndSubscriber) {
                receiver = EventReceiver.ItemSubscriber;
                SendEventData(receiver, eventData, sendParameters);
            }*/
            SendEventData(receiver, eventData, sendParameters);
        }

        public void SendInvisibilityChanged(EventReceiver receiver, bool invisibleValue) {
            ItemGeneric eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.InvisibilityChanged,
                EventData = invisibleValue
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            EventData eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendEventData(receiver, eventData, sendParameters);
        }

        private int healID = 1;
        public void SendHeal(EventReceiver receiver, Hashtable healProperties) {
            if(!nebulaObject) {
                return;
            }
            healProperties.Add((int)SPC.ShotID, healID);
            healID++;
            if(healID == int.MaxValue) {
                healID = 1;
            }
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.Heal,
                EventData = healProperties
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendEventData(receiver, eventData, sendParameters);
        }

        public void SendTargetUpdate() {
            if(!nebulaObject) { return;  }
            var evt = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.TargetUpdate,
                EventData = GetComponent<PlayerTarget>().GetInfo()
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, evt);
            ReceiveEvent(eventData, sendParameters);
        }

        /// <summary>
        /// Send model changed event when raceable model changed own model
        /// </summary>
        /// <param name="model"></param>
        public void SendModelChanged(string model) {
            var evt = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.ModelChanged,
                EventData = model
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, evt);
            SendEventData(EventReceiver.ItemSubscriber, eventData, sendParameters);
        }

        public void SendUnderConstructChanged(bool underConstruct) {
            var evt = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.UnderConstructChanged,
                EventData = underConstruct
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, evt);
            SendEventData(EventReceiver.ItemSubscriber, eventData, sendParameters);
        }


        public void PublishMove(float[] fromPos, float[] fromRot, float[] toPos, float[] toRot, float speed) {
            if(nebulaObject) {
                var eventInstance = new ItemMoved {
                    ItemId = nebulaObject.Id,
                    ItemType = nebulaObject.Type,
                    OldPosition = fromPos,
                    Position = toPos,
                    OldRotation = fromRot,
                    Rotation = toRot,
                    Speed = speed
                };

                var eventData = new EventData((byte)EventCode.ItemMoved, eventInstance);
                SendParameters sendParameters = new SendParameters { ChannelId = Settings.ItemEventChannel, Unreliable = true };
                var message = new ItemEventMessage(nebulaObject as Item, eventData, sendParameters);
                (nebulaObject as Item).EventChannel.Publish(message);
            }
        }




        private bool ReceiveEvent(EventData eventData, SendParameters sendParameters) {
            var player = GetComponent<MmoActor>();
            if(player != null) {
                player.Peer.SendEvent(eventData, sendParameters);
            }
            return true;
        }

        public void OnInvisibilityChanged(bool invisibleValue) {
            log.InfoFormat("MmoMessageComponent: invisibility changed {0}:{1}:{2} yellow",
                (ItemType)nebulaObject.Type, nebulaObject.Id, invisibleValue);
            EventReceiver receiver = EventReceiver.ItemSubscriber;
            if(nebulaObject.IsPlayer()) {
                receiver = EventReceiver.OwnerAndSubscriber;
            }
            SendInvisibilityChanged(receiver, invisibleValue);
        }

        //=================Contract Events===================================

        public void ContractsUpdate(Hashtable contractsHash) {
            if (false == nebulaObject) {
                return;
            }
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.ContractsUpdate,
                EventData = contractsHash
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            ReceiveEvent(eventData, sendParameters);
        }

        public void ContractDeclined(BaseContract contract) {
            ContractEvent(contract, CustomEventCode.ContractDeclined);
        }
        public void ContractAccepted(BaseContract contract) {
            ContractEvent(contract, CustomEventCode.ContractAccepted);
        }

        public void ContractCompleted(BaseContract contract) {
            ContractEvent(contract, CustomEventCode.ContractCompleted);
        }

        public void ContractReady(BaseContract contract) {
            ContractEvent(contract, CustomEventCode.ContractReady);
        }

        public void ContractStageChanged(BaseContract contract) {
            ContractEvent(contract, CustomEventCode.ContractStageChanged);
        }
        private void ContractEvent(BaseContract contract, CustomEventCode code) {
            if(null == nebulaObject) {
                return;
            }
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)code,
                EventData = contract.GetInfo()
            };
            SendParameters sendParameters = new SendParameters {
                 ChannelId = Settings.ItemEventChannel,
                  Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            ReceiveEvent(eventData, sendParameters);
        }

        public void ReceiveItemsAdded(InventoryType inventoryType) {
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.ItemsAdded,
                EventData = new Hashtable {
                    { (int)SPC.Inventory, (byte)inventoryType }
                }
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            ReceiveEvent(eventData, sendParameters);
        }

        public void ReceiveAchievmentUnlocked(string achievmentId, int tierId ) {
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.AchievmentUnlocked,
                EventData = new Hashtable {
                    { (int)SPC.AchievmentId,        achievmentId },
                    { (int)SPC.AchievmentTierId,    tierId }
                }
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            ReceiveEvent(eventData, sendParameters);
        }

        public void ReceivePlayerMark(Hashtable hash) {
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.ReceivePlayerMark,
                EventData = hash
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            ReceiveEvent(eventData, sendParameters);
        }

        public void FoundLoreRecord(string id) {
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                CustomEventCode = (byte)CustomEventCode.FoundLoreRecord,
                EventData = id
            };
            SendParameters sendParameters = new SendParameters {
                ChannelId = Settings.ItemEventChannel,
                Unreliable = false
            };
            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            ReceiveEvent(eventData, sendParameters);
        }
        //===================================================================
        public override int behaviourId {
            get {
                return (int)ComponentID.MmoMessager;
            }
        }


    }
}
