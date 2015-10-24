using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class MoveItemFromBankToStationOperationHandler : BaseOperationHandler {
        public MoveItemFromBankToStationOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            MoveItemFromBankToStationRequest operation = new MoveItemFromBankToStationRequest(peer.Protocol, request);
            if(!operation.IsValid) {
                return InvalidParametersOperationResponse(operation);
            }

            var success = peer.MoveItemToStation(operation.item, operation.count, operation.server);
            BaseSuccessResponse responseObject = new BaseSuccessResponse {
                success = success
            };
            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
