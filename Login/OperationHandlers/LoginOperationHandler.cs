using Common;
using Login.Operations;
using Photon.SocketServer;
using ServerClientCommon;

namespace Login.OperationHandlers {
    public class LoginOperationHandler : BaseOperationHandler  {
        public LoginOperationHandler(LoginApplication app, PeerBase peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            LoginOperationRequest operation = new LoginOperationRequest(peer.Protocol, request);
            if(!operation.IsValid) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.OperationInvalid,
                    DebugMessage = "Login operation parameters invalid"
                };
            }
            operation.Prepare();

            string password = StringChiper.Decrypt(operation.encryptedPassword);

            OperationResponse response;
            LoginReturnCode returnCode = LoginReturnCode.Ok;
            var user = application.GetExistingUser(operation.login, password, out returnCode);
            if(returnCode != LoginReturnCode.Ok ) {
                var responseObject = new LoginOperationResponse {
                    Login = operation.login,
                    GameRefId = string.Empty,
                    returnCode = (int)returnCode
                };
                response = new OperationResponse(request.OperationCode, responseObject);
            } else {
                var responseObject = new LoginOperationResponse {
                    Login = operation.login,
                    GameRefId = user.gameRef,
                    returnCode = (int)returnCode
                };
                response = new OperationResponse(request.OperationCode, responseObject);
                application.LogedInUsers.OnUserLoggedIn(operation.login, user.gameRef, peer as LoginClientPeer );
            }
            return response;
        }
    }
}
