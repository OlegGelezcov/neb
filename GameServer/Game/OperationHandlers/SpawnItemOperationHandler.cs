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
    public class SpawnItemOperationHandler : BasePlayerOperationHandler {
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            //var operation = new SpawnItem(actor.Peer.Protocol, request);
            //if (!operation.IsValid) {
            //    return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            //}

            //operation.OnStart();
            //InterestArea interestArea;
            //if (operation.InterestAreaId.HasValue) {
            //    if (actor.TryGetInterestArea(operation.InterestAreaId.Value, out interestArea) == false) {
            //        return operation.GetOperationResponse((int)ReturnCode.InterestAreaNotFound, "InterestAreaNotFound");
            //    }
            //} else {
            //    interestArea = null;
            //}

            //IWorld world = actor.World;
            //var item = new MmoItem(world, operation.Position, operation.Rotation, operation.Properties, actor, operation.ItemId, operation.ItemType);
            //if (world.ItemCache.AddItem(item)) {
            //    actor.AddItem(item);
            //    return this.ItemOperationSpawn(item, operation, interestArea, actor);
            //}

            //item.Dispose();
            //return operation.GetOperationResponse((int)ReturnCode.ItemAlreadyExists, "ItemAlreadyExists");
            return new OperationResponse(request.OperationCode);
        }

        private OperationResponse ItemOperationSpawn(MmoItem item, SpawnItem operation, InterestArea interestArea, MmoActor actor) {
            // this should always return Ok
            //MethodReturnValue result = this.CheckAccess(item, actor);

            //if (result) {
            //    item.Rotation = operation.Rotation;
            //    item.Spawn(operation.Position);

            //    if (interestArea != null) {
            //        lock (interestArea.SyncRoot) {
            //            interestArea.SubscribeItem(item);
            //        }
            //    }
            //}

            //operation.OnComplete();
            //return operation.GetOperationResponse(result);
            return operation.GetOperationResponse(MethodReturnValue.Ok);
        }
    }
}
