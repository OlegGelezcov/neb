using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using SelectCharacter.Operations;
using Common;

namespace SelectCharacter.OperationHandlers {
    public class BuyPvpItemOperationHandler : BaseOperationHandler {

        public BuyPvpItemOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            BuyPvpItemOperationRequest operation = new BuyPvpItemOperationRequest(peer.Protocol, request);
            if(!operation.IsValid ) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            RPCErrorCode errorCode;
            application.pvpStore.BuyStoreItem(operation.login, operation.gameRef, operation.character, operation.race, operation.workshop, operation.level, operation.type, operation.server, out errorCode);
            BuyPvpItemOperationResponse responseObject = new BuyPvpItemOperationResponse {
                 returnCode = (int)errorCode
            };
            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
