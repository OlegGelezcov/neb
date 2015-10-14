using Common;
using Photon.SocketServer;
using SelectCharacter.Characters;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class SelectCharacterOperationHandler : BaseOperationHandler {
        public SelectCharacterOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            SelectCharacterOperationRequest operation = new SelectCharacterOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            DbPlayerCharactersObject playerObject;
            if (false == this.application.Players.SelectCharacter(operation.GameRefId, operation.CharacterId, out playerObject)) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.Fatal,
                    DebugMessage = "Error of selecting character"
                };
            }

            SelectCharacterOperationResponse responseObject = new SelectCharacterOperationResponse {
                CharacterId = playerObject.SelectedCharacterId,
                Characters = playerObject.GetInfo()
            };
            peer.SetCharacterId(playerObject.SelectedCharacterId);

            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
