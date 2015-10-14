using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using Space.Game;
using Space.Server.Operations;
using Common;
using Space.Server;

namespace Nebula.Game.OperationHandlers {
    public class AttachInterestAreaOperationHandler : BasePlayerOperationHandler {
        /// <summary>
        ///   Handles operation <see cref = "AttachInterestArea" />: Attaches an existing <see cref = "InterestArea" /> to an existing <see cref = "Item" />.
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
        ///   An <see cref = "OperationResponse" /> with error code <see cref = "ReturnCode.Ok" />, <see cref = "ReturnCode.InterestAreaNotFound" /> or <see cref = "ReturnCode.ItemNotFound" />.
        /// </returns>
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new AttachInterestArea(actor.Peer.Protocol, request);
            if (!operation.IsValid) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            InterestArea interestArea;
            if (actor.TryGetInterestArea(operation.InterestAreaId, out interestArea) == false) {
                return operation.GetOperationResponse((int)ReturnCode.InterestAreaNotFound, "InterestAreaNotFound");
            }

            Item item;
            bool actorItem;
            if (false == operation.ItemType.HasValue || string.IsNullOrEmpty(operation.ItemId)) {
                item = actor.Avatar;
                actorItem = true;

                // set return vaues
                operation.ItemId = item.Id;
                operation.ItemType = item.Type;
            } else {
                IWorld world = actor.World;
                actorItem = actor.TryGetItem(operation.ItemType.Value, operation.ItemId, out item);
                if (actorItem == false) {
                    if (world.ItemCache.TryGetItem(operation.ItemType.Value, operation.ItemId, out item) == false) {
                        return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                    }
                }
            }

            if (actorItem) {
                // we are already in the item thread, invoke directly
                return this.ItemOperationAttachInterestArea(actor, item, operation, interestArea, sendParameters);
            }

            // second parameter (peer) allows us to send an error event to the client (in case of an error)
            item.Fiber.Enqueue(
                () => actor.ExecItemOperation(() => this.ItemOperationAttachInterestArea(actor, item, operation, interestArea, sendParameters), sendParameters));

            // response is sent later
            return null;
        }

        private OperationResponse ItemOperationAttachInterestArea(MmoActor actor, Item item, AttachInterestArea operation, InterestArea interestArea, SendParameters sendParameters) {
            if (item.Disposed) {
                return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            lock (interestArea.SyncRoot) {
                interestArea.Detach();
                interestArea.AttachToItem(item);
                interestArea.UpdateInterestManagement();
            }

            // use item channel to ensure that this event arrives before any move or subscribe events
            OperationResponse response = operation.GetOperationResponse(MethodReturnValue.Ok);
            sendParameters.ChannelId = Settings.ItemEventChannel;
            actor.Peer.SendOperationResponse(response, sendParameters);

            operation.OnComplete();
            return null;
        }
    }
}
