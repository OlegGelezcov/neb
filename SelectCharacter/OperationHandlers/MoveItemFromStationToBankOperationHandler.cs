using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class MoveItemFromStationToBankOperationHandler : BaseOperationHandler {
        public MoveItemFromStationToBankOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            MoveItemFromStationToBankRequest operation = new MoveItemFromStationToBankRequest(peer.Protocol, request);
            if(!operation.IsValid ) {
                return InvalidParametersOperationResponse(operation);
            }

            bool status = peer.MoveItemFromStation(operation.item, operation.count, operation.server);
            BaseSuccessResponse responseObject = new BaseSuccessResponse {
                success = status
            };
            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
