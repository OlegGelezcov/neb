using Nebula.Server.Operations;
using Photon.SocketServer;
using Space.Game;

namespace Nebula.Game.OperationHandlers {
    public class RequestServerIDOperationHandler : BasePlayerOperationHandler {
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new RequestServerID(actor.Peer.Protocol, request);
            RequestServerIDResponse responseObject = new RequestServerIDResponse {
                id = GameApplication.ServerId.ToString()
            };
            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
