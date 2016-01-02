using Common;
using Login.Operations;
using Nebula.Server.Login;
using Photon.SocketServer;

namespace Login.OperationHandlers {
    public class GetNebulaCreditsOperationHandler : BaseOperationHandler  {
        public GetNebulaCreditsOperationHandler(LoginApplication app, PeerBase peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            GetNebulaCreditsRequest operation = new GetNebulaCreditsRequest(peer.Protocol, request);
            if(!operation.IsValid) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.InvalidOperationParameter,
                    DebugMessage = operation.GetErrorMessage()
                };
            }

            GameRefId gameRef = new GameRefId(operation.gameRef);
            var user = application.GetUser(gameRef);

            if(user == null ) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.UserNotFound,
                    DebugMessage = string.Format("user with game ref = {0} not found in database", gameRef.value)
                };
            }

            GetNebulaCreditsResponse responseData = new GetNebulaCreditsResponse {
                 count = user.nebulaCredits
            };

            return new OperationResponse(request.OperationCode, responseData);
        }
    }
}
