using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using Space.Server.Operations;
using Common;
using Space.Game;
using Space.Server;

namespace Nebula.Game.OperationHandlers {

    public class AddInterestAreaOperationHandler : BasePlayerOperationHandler{

        /// <summary>
        ///   Handles operation <see cref = "AddInterestArea" />: Creates a new <see cref = "InterestArea" /> and optionally attaches it to an existing <see cref = "Item" />.
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
        ///   An <see cref = "OperationResponse" /> with error code <see cref = "ReturnCode.Ok" /> or <see cref = "ReturnCode.InterestAreaAlreadyExists" />.
        ///   If the <see cref = "InterestArea" /> is supposed to be attached to an <see cref = "Item" /> error code <see cref = "ReturnCode.ItemNotFound" /> could be returned. 
        /// </returns>
        /// <remarks>
        ///   The <see cref = "InterestArea" /> is created even if error code <see cref = "ReturnCode.ItemNotFound" /> is returned.
        /// </remarks>
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new AddInterestArea(actor.Peer.Protocol, request);
            if (!operation.IsValid) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (int)ReturnCode.InvalidOperationParameter,
                    DebugMessage = operation.GetErrorMessage()
                };
            }

            operation.OnStart();
            InterestArea interestArea;
            if (actor.TryGetInterestArea(operation.InterestAreaId, out interestArea)) {
                return operation.GetOperationResponse((int)ReturnCode.InterestAreaAlreadyExists, "InterestAreaAlreadyExists");
            }

            interestArea = new MmoClientInterestArea(actor.Peer, operation.InterestAreaId, actor.World);
            actor.AddInterestArea(interestArea);

            // attach interestArea to item
            Item item;
            if (operation.ItemType.HasValue && string.IsNullOrEmpty(operation.ItemId) == false) {
                IWorld world = actor.World;

                bool actorItem = actor.TryGetItem(operation.ItemType.Value, operation.ItemId, out item);
                if (actorItem == false) {
                    if (world.ItemCache.TryGetItem(operation.ItemType.Value, operation.ItemId, out item) == false) {
                        return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                    }
                }

                if (actorItem) {
                    // we are already in the item thread, invoke directly
                    return ItemOperationAddInterestArea(item, operation, interestArea);
                }

                // second parameter (peer) allows us to send an error event to the client (in case of an error)
                item.Fiber.Enqueue(() => actor.ExecItemOperation(() => ItemOperationAddInterestArea(item, operation, interestArea), sendParameters));

                // send response later
                return null;
            }

            // free floating interestArea
            if (operation.Position != null) {
                lock (interestArea.SyncRoot) {
                    interestArea.Position = operation.Position.ToVector(true);
                    interestArea.ViewDistanceEnter = operation.ViewDistanceEnter.ToVector(true);
                    interestArea.ViewDistanceExit = operation.ViewDistanceExit.ToVector(true);
                    interestArea.UpdateInterestManagement();
                }
            }

            return operation.GetOperationResponse(MethodReturnValue.Ok);
        }

        private static OperationResponse ItemOperationAddInterestArea(Item item, AddInterestArea operation, InterestArea interestArea) {
            if (item.Disposed) {
                return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            lock (interestArea.SyncRoot) {
                interestArea.AttachToItem(item);
                interestArea.ViewDistanceEnter = operation.ViewDistanceEnter.ToVector(true);
                interestArea.ViewDistanceExit = operation.ViewDistanceExit.ToVector(true);
                interestArea.UpdateInterestManagement();
            }

            operation.OnComplete();
            return operation.GetOperationResponse(MethodReturnValue.Ok);
        }
    }
}
