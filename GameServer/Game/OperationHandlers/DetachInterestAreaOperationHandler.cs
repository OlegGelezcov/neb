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

    /// <summary>
    ///   Handles operation <see cref = "DetachInterestArea" />: Detaches an existing <see cref = "InterestArea" /> from an <see cref = "Item" />.
    /// </summary>
    /// <param name = "peer">
    ///   The client peer.
    /// </param>
    /// <param name = "request">
    ///   The request.
    /// </param>
    /// <returns>
    ///   An <see cref = "OperationResponse" /> with error code <see cref = "ReturnCode.Ok" /> or <see cref = "ReturnCode.InterestAreaNotFound" />.
    /// </returns>
    public class DetachInterestAreaOperationHandler : BasePlayerOperationHandler {
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new DetachInterestArea(actor.Peer.Protocol, request);
            if (!operation.IsValid) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            InterestArea interestArea;
            if (actor.TryGetInterestArea(operation.InterestAreaId, out interestArea) == false) {
                return operation.GetOperationResponse((int)ReturnCode.InterestAreaNotFound, "InterestAreaNotFound");
            }

            lock (interestArea.SyncRoot) {
                interestArea.Detach();
            }

            return operation.GetOperationResponse(MethodReturnValue.Ok);
        }
    }
}
