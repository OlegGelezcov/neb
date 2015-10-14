using Common;
using ExitGames.Logging;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class SetNewPriceOperationHandler : BaseOperationHandler {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public SetNewPriceOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            SetNewPriceOperationRequest operation = new SetNewPriceOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse(SelectCharacterOperationCode.SetNewPrice);
            }
            bool status = application.Stores.SetNewPrice(operation.login, operation.gameRefID, operation.characterID, operation.storeItemID, operation.price);
            log.InfoFormat("new price setted with status = {0}", status);
            SetNewPriceOperationResponse response = new SetNewPriceOperationResponse { status = status };
            return new OperationResponse((byte)SelectCharacterOperationCode.SetNewPrice, response);
        }
    }
}
