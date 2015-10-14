using Common;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class ExitGuildOperationHandler : BaseOperationHandler {

        public ExitGuildOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            ExitGuildOperationRequest operation = new ExitGuildOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) { return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode); }
            bool success = application.Guilds.RemoveMember(operation.Login, operation.CharacterID, operation.GuildID);
            ExitGuildOperationResponse response = new ExitGuildOperationResponse { Result = success };
            return new OperationResponse(request.OperationCode, response);
        }
    }
}
