using Login.Operations;
using Photon.SocketServer;

namespace Login.OperationHandlers {
    public class GetUsersOnlineHandler : BaseOperationHandler {

        public GetUsersOnlineHandler(LoginApplication app, PeerBase peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            GetUserOnlineResponse responseObject = new GetUserOnlineResponse {
                count = application.LogedInUsers.Count
            };
            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
