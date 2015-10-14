using Common;
using Photon.SocketServer;
using SelectCharacter.Characters;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class DeleteCharacterOperationHandler : BaseOperationHandler {
        public DeleteCharacterOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            DeleteCharacterOperationRequest operation = new DeleteCharacterOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            if (string.IsNullOrEmpty(operation.GameRefId) || string.IsNullOrEmpty(operation.CharacterId)) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.InvalidOperationParameter,
                    DebugMessage = "GameRefId or CharacterId invalid operation parameter"
                };
            }

            DbPlayerCharactersObject updatedPlayerCharactersObject = null;
            if (!this.application.Players.DeleteCharacter(operation.GameRefId, operation.CharacterId, out updatedPlayerCharactersObject)) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.Fatal,
                    DebugMessage = "Error of deleting character"
                };
            }

            if (updatedPlayerCharactersObject == null) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.Fatal,
                    DebugMessage = "Error of receiving PlayerCharactersObject"
                };
            }

            DeleteCharacterOperationResponse responseObject = new DeleteCharacterOperationResponse {
                Characters = updatedPlayerCharactersObject.GetInfo()
            };

            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
