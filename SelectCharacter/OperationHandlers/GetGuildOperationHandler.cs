using Common;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class GetGuildOperationHandler : BaseOperationHandler {

        public GetGuildOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            GetGuildOperationRequest operation = new GetGuildOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            var player = application.Players.GetExistingPlayer(operation.GameRefID);
            if (player == null) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.Fatal, DebugMessage = "Player not exists" };
            }

            var character = player.Data.GetCharacter(operation.CharacterID);
            if (character == null) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.Fatal, DebugMessage = "Character not exists" };
            }

            string guildID = character.guildID;
            if (string.IsNullOrEmpty(guildID)) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.InvalidOperation, DebugMessage = "Guild not exists" };
            }

            var guild = application.Guilds.GetGuild(guildID);
            if (guild == null) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.Fatal, DebugMessage = "Guild not founded" };
            }

            //if(operation.page == PageDirection.NEXT) {
            //    guild.IncrementPage();
            //} else if(operation.page == PageDirection.PREV) {
            //    guild.DecrementPage();
            //}
            GetGuildOperationResponse response = new GetGuildOperationResponse { GuildInfo = guild.GetInfo(application) };
            return new OperationResponse(request.OperationCode, response);
        }
    }
}
