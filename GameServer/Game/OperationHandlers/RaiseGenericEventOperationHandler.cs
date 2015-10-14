using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using Space.Game;
using Space.Server.Operations;
using Common;
using Space.Server;
using Space.Server.Events;
using System.Collections;
using ServerClientCommon;
using Space.Server.Messages;
using ExitGames.Logging;
using Nebula.Game.Components;

namespace Nebula.Game.OperationHandlers {
    public class RaiseGenericEventOperationHandler : BasePlayerOperationHandler {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
         
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            try {
                var operation = new RaiseGenericEvent(actor.Peer.Protocol, request);
                if (!operation.IsValid) {
                    return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
                }

                operation.OnStart();
                Item item;
                bool actorItem = true;
                if (false == operation.ItemType.HasValue || string.IsNullOrEmpty(operation.ItemId)) {
                    item = actor.Avatar;

                    // set return values
                    operation.ItemType = item.Type;
                    operation.ItemId = item.Id;
                } else if (actor.TryGetItem(operation.ItemType.Value, operation.ItemId, out item) == false) {
                    if (actor.World.ItemCache.TryGetItem(operation.ItemType.Value, operation.ItemId, out item) == false) {
                        return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                    }

                    actorItem = false;
                }

                if (actorItem) {
                    // we are already in the item thread, invoke directly
                    return ItemOperationRaiseGenericEvent(item, operation, sendParameters);
                }

                // second parameter (peer) allows us to send an error event to the client (in case of an error)
                item.Fiber.Enqueue(() => actor.ExecItemOperation(() => ItemOperationRaiseGenericEvent(item, operation, sendParameters), sendParameters));

                // operation continued later
                return null;
            } catch (Exception ex) {
                log.Error(ex);
            }
            return null;
        }

        private static OperationResponse ItemOperationRaiseGenericEvent(Item item, RaiseGenericEvent operation, SendParameters sendParameters) {
            try {
                if (item.Disposed) {
                    return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                }

                var eventInstance = new ItemGeneric {
                    ItemId = item.Id,
                    ItemType = item.Type,
                    CustomEventCode = operation.CustomEventCode,
                    EventData = operation.EventData,
                };

                var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
                sendParameters.Unreliable = (Reliability)operation.EventReliability == Reliability.Unreliable;
                sendParameters.ChannelId = Settings.ItemEventChannel;



                //switch ((CustomEventCode)operation.CustomEventCode) {
                //    case CustomEventCode.Fire:
                //        {
                //            Hashtable hashSource = eventInstance.EventData as Hashtable;
                //            //ShotType shotType = (ShotType)hashSource.Value<byte>((int)SPC.ShotType, (byte)ShotType.Light);
                //            WeaponHitInfo hit;
                //            Hashtable shotInfo = item.GetComponent<BaseWeapon>().Fire(out hit);
                //            item.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);


                //            if (hit.hitAllowed && !hit.IsWeaponBlocked && hit.isHitted) {
                //                var damagable = item.GetComponent<ShipBasedDamagableObject>();
                //                var bonus = item.GetComponent<PlayerBonuses>();
                //            }
                //        }

                //        break;
                //}

                if ((CustomEventCode)operation.CustomEventCode != CustomEventCode.Fire) {

                    switch (operation.EventReceiver) {
                        case (byte)EventReceiver.ItemOwner:
                            {
                                if (((IMmoItem)item).ReceiveEvent(eventData, sendParameters) == false) {
                                    string debugMessage = string.Format("Target item {0}/{1} could not receive event", item.Type, item.Id);
                                    operation.OnComplete();
                                    return operation.GetOperationResponse((int)ReturnCode.InvalidOperation, debugMessage);
                                }

                                break;
                            }

                        case (byte)EventReceiver.ItemSubscriber:
                            {
                                var message = new ItemEventMessage(item, eventData, sendParameters);
                                item.EventChannel.Publish(message);
                                break;
                            }
                        case (byte)EventReceiver.OwnerAndSubscriber:
                            {
                                ((IMmoItem)item).ReceiveEvent(eventData, sendParameters);
                                var message = new ItemEventMessage(item, eventData, sendParameters);
                                item.EventChannel.Publish(message);
                                break;
                            }
                        default:
                            {
                                operation.OnComplete();
                                return operation.GetOperationResponse((int)ReturnCode.ParameterOutOfRange, "Invalid EventReceiver " + operation.EventReceiver);
                            }
                    }
                }

                // no response
                
                return null;
            } catch (Exception ex) {
                log.Error(ex);
            }
            operation.OnComplete();
            return null;
        }

    }
}
