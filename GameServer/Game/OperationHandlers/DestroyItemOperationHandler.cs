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

namespace Nebula.Game.OperationHandlers {
    public class DestroyItemOperationHandler : BasePlayerOperationHandler {

        /// <summary>
        ///   Handles operation <see cref = "DestroyItem" />: Destroys an existing <see cref = "MmoItem" />. 
        ///   The <see cref = "MmoItem.Owner" /> must be this actor instance.
        /// </summary>
        /// <param name = "peer">
        ///   The client peer.
        /// </param>
        /// <param name = "request">
        ///   The request.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        /// <returns>
        ///   An <see cref = "OperationResponse" /> with error code <see cref = "ReturnCode.Ok" />, <see cref = "ReturnCode.ItemNotFound" /> or <see cref = "ReturnCode.ItemAccessDenied" />.
        /// </returns>
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new DestroyItem(actor.Peer.Protocol, request);
            if (!operation.IsValid) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            Item item;
            bool actorItem = actor.TryGetItem(operation.ItemType, operation.ItemId, out item);
            if (actorItem == false) {
                IWorld world = actor.World;

                // search world cache just to see if item exists at all
                if (world.ItemCache.TryGetItem(operation.ItemType, operation.ItemId, out item) == false) {
                    return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                }
            }

            if (actorItem) {
                // we are already in the item thread, invoke directly
                return ItemOperationDestroy(actor, item, operation);
            }

            // second parameter (peer) allows us to send an error event to the client (in case of an error)
            // error ItemAccessDenied or ItemNotFound will be returned
            item.Fiber.Enqueue(() => actor.ExecItemOperation(() => this.ItemOperationDestroy(actor, item, operation), sendParameters));

            // operation is continued later
            return null;
        }

        private OperationResponse ItemOperationDestroy(MmoActor actor, Item item, DestroyItem operation) {
            MethodReturnValue result = this.CheckAccess(item, actor);
            if (result) {
                item.Destroy();
                item.Dispose();
                actor.RemoveItem(item);

                (item.world as MmoWorld).ItemCache.RemoveItem(item.Type, item.Id);
                var eventInstance = new ItemDestroyed { ItemId = item.Id, ItemType = item.Type };
                var eventData = new EventData((byte)EventCode.ItemDestroyed, eventInstance);
                actor.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });

                // no response, event is sufficient
                operation.OnComplete();
                return null;
            }

            return operation.GetOperationResponse(result);
        }


    }
}
