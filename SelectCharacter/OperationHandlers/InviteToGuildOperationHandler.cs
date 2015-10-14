using Common;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class InviteToGuildOperationHandler : BaseOperationHandler {

        public InviteToGuildOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            InviteGuildOperationRequest operation = new InviteGuildOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            if (!application.Guilds.SendInviteToGuildNotification(operation.SourceLogin, operation.SourceCharacterId, operation.TargetLogin, operation.TargetCharacterId, operation.GuildID)) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.Fatal, DebugMessage = "Some error occured" };
            } else {
                return new OperationResponse(request.OperationCode);
            }
        }
    }
}
