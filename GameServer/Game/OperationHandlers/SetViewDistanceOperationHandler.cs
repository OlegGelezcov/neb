using Common;
using ExitGames.Logging;
using Photon.SocketServer;
using Space.Game;
using Space.Server;
using Space.Server.Operations;

namespace Nebula.Game.OperationHandlers {
    class SetViewDistanceOperationHandler : BasePlayerOperationHandler {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new SetViewDistance(actor.Peer.Protocol, request);
            if (!operation.IsValid) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            InterestArea interestArea;
            if (actor.TryGetInterestArea(operation.InterestAreaId, out interestArea) == false) {
                return operation.GetOperationResponse((int)ReturnCode.InterestAreaNotFound, "InterestAreaNotFound");
            }


            lock (interestArea.SyncRoot) {
                interestArea.ViewDistanceEnter = operation.ViewDistanceEnter.ToVector(true);
                interestArea.ViewDistanceExit = operation.ViewDistanceExit.ToVector(true);
                interestArea.UpdateInterestManagement();
            }

            log.InfoFormat("setted interest area min = {0} max = {1}", interestArea.ViewDistanceEnter, interestArea.ViewDistanceExit);
            // don't send response
            return null;
        }
    }
}
