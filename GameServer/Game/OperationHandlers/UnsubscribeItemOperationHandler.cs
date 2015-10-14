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
    public class UnsubscribeItemOperationHandler : BasePlayerOperationHandler {
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new UnsubscribeItem(actor.Peer.Protocol, request);
            if (!operation.IsValid) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            InterestArea interestArea;
            if (actor.TryGetInterestArea(operation.InterestAreaId, out interestArea) == false) {
                return operation.GetOperationResponse((int)ReturnCode.InterestAreaNotFound, "InterestAreaNotFound");
            }

            IWorld world = actor.World;

            Item item;
            bool actorItem = actor.TryGetItem(operation.ItemType, operation.ItemId, out item);
            if (actorItem == false) {
                if (world.ItemCache.TryGetItem(operation.ItemType, operation.ItemId, out item) == false) {
                    return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                }
            }

            lock (interestArea.SyncRoot) {
                interestArea.UnsubscribeItem(item);
            }

            // don't send response
            return null;
        }
    }
}
