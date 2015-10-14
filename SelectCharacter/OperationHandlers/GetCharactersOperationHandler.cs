using Common;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class GetCharactersOperationHandler : BaseOperationHandler {
        public GetCharactersOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            GetCharactersOperationRequest operation = new GetCharactersOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            string gameRefId = operation.GameRefId;
            if (string.IsNullOrEmpty(gameRefId)) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.InvalidOperationParameter,
                    DebugMessage = "GameRefId invalid operation parameter"
                };
            }

            var playerCharactersObject = this.application.Players.GetPlayerCharactersObject(gameRefId, operation.Login);
            peer.SetCharacterId(playerCharactersObject.SelectedCharacterId);

            GetCharactersOperationResponse responseObject = new GetCharactersOperationResponse {
                Characters = playerCharactersObject.GetInfo()
            };
            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
