using Common;
using Login.Operations;
using Photon.SocketServer;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login.OperationHandlers {
    public class UsePassOperationHandler : BaseOperationHandler {

        public UsePassOperationHandler(LoginApplication app, PeerBase peer) 
            : base (app, peer ) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {

            UsePassRequest operation = new UsePassRequest(peer.Protocol, request);
            if(!operation.IsValid) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.InvalidOperationParameter,
                    DebugMessage = operation.GetErrorMessage()
                };
            }
            operation.Prepare();

            var database = application.DbUserLogins;
            var dbUser = database.GetExistingUserForGameRef(operation.login, operation.gameRef);
            if(dbUser == null ) {
                UsePassResponse responseObject = new UsePassResponse {
                     returnCode = (int)LoginReturnCode.UserNotFound
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }

            if(dbUser.passes <= 0 ) {
                UsePassResponse responseObject = new UsePassResponse {
                    returnCode = (int)LoginReturnCode.NoPasses
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }

            dbUser.MoveSinglePassToTime();
            database.SaveUser(dbUser);

            application.LogedInUsers.SendPassesUpdateEvent(dbUser);

            //send pass update to client
            UsePassResponse successResponseObject = new UsePassResponse {
                returnCode = (int)LogicErrorCode.OK
            };
            return new OperationResponse(request.OperationCode, successResponseObject);
        }
    }
}
