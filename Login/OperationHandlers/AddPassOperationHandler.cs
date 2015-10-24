using Common;
using Login.Operations;
using Photon.SocketServer;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login.OperationHandlers {
    public class AddPassOperationHandler : BaseOperationHandler  {
        public AddPassOperationHandler(LoginApplication app, PeerBase peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            AddPassRequest operation = new AddPassRequest(peer.Protocol, request);
            if(!operation.IsValid) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.InvalidOperationParameter,
                    DebugMessage = operation.GetErrorMessage()
                };
            }

            var database = application.DbUserLogins;
            var dbUser = database.GetExistingUserForGameRef(operation.login, operation.gameRef);

            if(dbUser == null ) {
                AddPassOperationResponse responseObject = new AddPassOperationResponse {
                    returnCode = (int)LoginReturnCode.UserNotFound
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }
            dbUser.IncrementPasses();
            database.SaveUser(dbUser);
            application.LogedInUsers.SendPassesUpdateEvent(dbUser);

            AddPassOperationResponse successResponseObject = new AddPassOperationResponse {
                returnCode = (int)LoginReturnCode.Ok
            };
            return new OperationResponse(request.OperationCode, successResponseObject);
        }
    }
}
