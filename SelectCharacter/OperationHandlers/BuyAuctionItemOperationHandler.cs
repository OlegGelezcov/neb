using Common;
using ExitGames.Logging;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class BuyAuctionItemOperationHandler : BaseOperationHandler {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public BuyAuctionItemOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            BuyAuctionItemOperationRequest operation = new BuyAuctionItemOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse(SelectCharacterOperationCode.BuyAuctionItem);
            }

            bool status = application.Stores.BuyAuctionItem(operation.login, operation.gameRefID, operation.characterID, operation.storeItemID);
            log.InfoFormat("application HandleBuyAuctionItem status = {0}", status);
            BuyAuctionItemOperationResponse response = new BuyAuctionItemOperationResponse { status = status };
            return new OperationResponse((byte)SelectCharacterOperationCode.BuyAuctionItem, response);
        }
    }
}
