using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using Common;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class GetPlayerStoreOperationHandler : BaseOperationHandler {
        public GetPlayerStoreOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {

            GetPlayerStoreOperationRequest operation = new GetPlayerStoreOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            var store = application.Stores.GetOrCreatePlayerStore(operation.login, operation.gameRefID, operation.characterID);
            if (store == null) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.PlayerStoreNotFounded, DebugMessage = "not founded player store" };
            }

            GetPlayerStoreOperationResponse response = new GetPlayerStoreOperationResponse { info = store.GetInfo() };
            return new OperationResponse(request.OperationCode, response);
        }
    }
}
