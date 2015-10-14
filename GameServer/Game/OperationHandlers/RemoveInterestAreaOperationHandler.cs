using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using Space.Game;
using Space.Server.Operations;
using Space.Server;
using Common;

namespace Nebula.Game.OperationHandlers {
    public class RemoveInterestAreaOperationHandler : BasePlayerOperationHandler {
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new RemoveInterestArea(actor.Peer.Protocol, request);
            if (!operation.IsValid) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            InterestArea interestArea;
            if (actor.TryGetInterestArea(operation.InterestAreaId, out interestArea)) {
                lock (interestArea.SyncRoot) {
                    interestArea.Detach();
                    interestArea.Dispose();
                }

                actor.RemoveInterestArea(operation.InterestAreaId);
                return operation.GetOperationResponse(MethodReturnValue.Ok);
            }

            return operation.GetOperationResponse((int)ReturnCode.InterestAreaNotFound, "InterestAreaNotFound");
        }
    }
}
