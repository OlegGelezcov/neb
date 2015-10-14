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
    public class GetPropertiesOperationHandler : BasePlayerOperationHandler {
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new GetProperties(actor.Peer.Protocol, request);
            if (!operation.IsValid) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            IWorld world = actor.World;
            Item item;
            bool actorItem = actor.TryGetItem(operation.ItemType, operation.ItemId, out item);
            if (actorItem == false) {
                if (world.ItemCache.TryGetItem(operation.ItemType, operation.ItemId, out item) == false) {
                    return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                }
            }

            if (actorItem) {
                // we are already in the item thread, invoke directly
                return this.ItemOperationGetProperties(item, operation, actor);
            }

            // second parameter (peer) allows us to send an error event to the client (in case of an error)
            item.Fiber.Enqueue(() => actor.ExecItemOperation(() => this.ItemOperationGetProperties(item, operation, actor), sendParameters));

            // operation is continued later
            return null;
        }

        private OperationResponse ItemOperationGetProperties(Item item, GetProperties operation, MmoActor actor) {
            if (item.Disposed) {
                return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            if (item.properties != null) {
                //if (operation.PropertiesRevision.HasValue == false || operation.PropertiesRevision.Value != item.PropertiesRevision)
                //{
                ItemProperties properties = null;
                properties = new ItemProperties {
                    ItemId = item.Id,
                    ItemType = item.Type,
                    PropertiesRevision = item.properties.propertiesRevision,
                    PropertiesSet = item.properties.raw
                };

                var eventData = new EventData((byte)EventCode.ItemProperties, properties);
                actor.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel, Unreliable = true });
                //}
            }

            // no response sent
            operation.OnComplete();
            return null;
        }

    }
}
