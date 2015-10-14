using Common;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class DeleteAuctionItemOperationHandler : BaseOperationHandler {

        public DeleteAuctionItemOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            RemoveAuctionItemOperationRequest operation = new RemoveAuctionItemOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse(SelectCharacterOperationCode.DeleteAuctionItem);
            }
            bool status = application.Stores.RemoveAuctionItem(operation.login, operation.gameRefID, operation.characterID, operation.storeItemID);
            RemoveAuctionItemOperationResponse response = new RemoveAuctionItemOperationResponse { status = status };
            return new OperationResponse((byte)SelectCharacterOperationCode.DeleteAuctionItem, response);
        }
    }
}
