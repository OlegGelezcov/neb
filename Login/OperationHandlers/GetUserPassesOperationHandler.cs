using Common;
using Login.Operations;
using Photon.SocketServer;
using ServerClientCommon;

namespace Login.OperationHandlers {
    public class GetUserPassesOperationHandler : BaseOperationHandler {

        public GetUserPassesOperationHandler(LoginApplication app, PeerBase peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            GetUserPassesRequest operation = new GetUserPassesRequest(peer.Protocol, request);
            if(!operation.IsValid) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.InvalidOperationParameter,
                    DebugMessage = operation.GetErrorMessage()
                };
            }

            var database = application.DbUserLogins;
            var dbUser = database.GetExistingUserForGameRef(operation.login, operation.gameRef);
            if(dbUser == null ) {
                GetUserPassesResponse responseObject = new GetUserPassesResponse {
                    currentTime = 0,
                    expireTime = 0,
                    passes = 0,
                    returnCode = (int)LoginReturnCode.UserNotFound
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }

            GetUserPassesResponse successResponseObject = new GetUserPassesResponse {
                currentTime = CommonUtils.SecondsFrom1970(),
                expireTime = dbUser.expireTime,
                passes = dbUser.passes,
                returnCode = (int)LoginReturnCode.Ok
            };
            return new OperationResponse(request.OperationCode, successResponseObject);
        }
    }
}
