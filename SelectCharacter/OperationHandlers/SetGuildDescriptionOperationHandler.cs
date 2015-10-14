using Common;
using Photon.SocketServer;
using SelectCharacter.Guilds;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class SetGuildDescriptionOperationHandler : BaseOperationHandler {

        public SetGuildDescriptionOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            SetGuildDescriptionOperationRequest operation = new SetGuildDescriptionOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            bool success = application.Guilds.SetGuildDescription(operation.CharacterID, operation.GuildID, operation.Description);
            if (!success) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.LogicError, DebugMessage = "Error of set description occured" };
            } else {
                Guild guild = application.Guilds.GetGuild(operation.GuildID);
                if (guild != null) {
                    return new OperationResponse(request.OperationCode, new SetGuildDescriptionOperationResponse { Guild = guild.GetInfo(application) });
                } else {
                    return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.Fatal, DebugMessage = "Guild not found" };
                }
            }
        }
    }
}
