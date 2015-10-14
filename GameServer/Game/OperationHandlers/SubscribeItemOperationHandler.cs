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

namespace Nebula.Game.OperationHandlers {
    public class SubscribeItemOperationHandler : BasePlayerOperationHandler {
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new SubscribeItem(actor.Peer.Protocol, request);
            if (!operation.IsValid) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            IWorld world = actor.World;
            InterestArea interestArea;
            if (actor.TryGetInterestArea(operation.InterestAreaId, out interestArea) == false) {
                return operation.GetOperationResponse((int)ReturnCode.InterestAreaNotFound, "InterestAreaNotFound");
            }

            Item item;
            bool actorItem = actor.TryGetItem(operation.ItemType, operation.ItemId, out item);
            if (actorItem == false) {
                if (world.ItemCache.TryGetItem(operation.ItemType, operation.ItemId, out item) == false) {
                    return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                }
            }

            if (actorItem) {
                // we are already in the item thread, invoke directly
                return this.ItemOperationSubscribeItem(item, operation, interestArea, actor);
            }

            // second parameter (peer) allows us to send an error event to the client (in case of an error)
            item.Fiber.Enqueue(() => actor.ExecItemOperation(() => this.ItemOperationSubscribeItem(item, operation, interestArea, actor), sendParameters));

            // operation continues later
            return null;
        }

        private OperationResponse ItemOperationSubscribeItem(Item item, SubscribeItem operation, InterestArea interestArea, MmoActor actor) {
            if (item.Disposed) {
                return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            lock (interestArea.SyncRoot) {
                interestArea.SubscribeItem(item);
            }

            if (operation.PropertiesRevision.HasValue == false || operation.PropertiesRevision.Value != item.properties.propertiesRevision) {
                var properties = new ItemPropertiesSet {
                    ItemId = item.Id,
                    ItemType = item.Type,
                    PropertiesRevision = item.properties.propertiesRevision,
                    PropertiesSet = item.properties.raw
                };
                var eventData = new EventData((byte)EventCode.ItemPropertiesSet, properties);
                actor.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });
            }

            // don't send response
            operation.OnComplete();
            return null;
        }
    }
}
