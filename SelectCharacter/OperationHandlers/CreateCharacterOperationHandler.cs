using Common;
using Photon.SocketServer;
using SelectCharacter.Characters;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class CreateCharacterOperationHandler : BaseOperationHandler {
        public CreateCharacterOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer ) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            CreateCharacterOperationRequest operation = new CreateCharacterOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            var characterName = application.DB.GetCharacterName(operation.DisplayName);
            if(characterName != null ) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.CharacterWithSameNameAlreadyExists,
                    DebugMessage = operation.DisplayName
                };
            }

            DbPlayerCharactersObject player = null;
            var response = this.application.Players.CreateCharacter(
                request, sendParameters, operation.GameRefId,
                operation.DisplayName, (Race)operation.Race,
                (Workshop)operation.Workshop, operation.icon, out player);
            if (player != null) {
                peer.SetCharacterId(player.SelectedCharacterId);
                application.DB.WriteCharacterName(operation.GameRefId, response.Parameters[(byte)ParameterCode.CharacterId].ToString(), operation.DisplayName);
            }

            

            return response;
        }
    }
}
