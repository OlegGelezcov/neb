using Common;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class DeleteGuildOperationHandler : BaseOperationHandler {

        public DeleteGuildOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            DeleteGuildOperationRequest operation = new DeleteGuildOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }
            bool success = application.Guilds.DeleteGuild(operation.SourceCharacterID);
            ExitGuildOperationResponse response = new ExitGuildOperationResponse {
                Result = success
            };
            return new OperationResponse(request.OperationCode, response);
        }
    }
}
