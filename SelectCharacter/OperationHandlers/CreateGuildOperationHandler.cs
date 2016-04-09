using Common;
using ExitGames.Logging;
using Photon.SocketServer;
using SelectCharacter.Guilds;
using SelectCharacter.Operations;
using ServerClientCommon;

namespace SelectCharacter.OperationHandlers {
    public class CreateGuildOperationHandler : BaseOperationHandler {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public CreateGuildOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            CreateGuildOperationRequest operation = new CreateGuildOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                log.InfoFormat(operation.GetErrorMessage());
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            //check the guild name is unique
            var existingGuild = application.Guilds.FindGuild(operation.Name);
            if (existingGuild != null) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.AlreadyMatched };
            }

            string characterName = string.Empty;
            int characterIcon = -1;
            var player = application.Players.GetExistingPlayer(operation.GameRefId);
            if(player != null ) {
                var character = player.Data.GetCharacter(operation.CharacterId);
                if(character != null ) {
                    characterName = character.Name;
                    characterIcon = character.characterIcon;
                }
            }
            GuildMember member = new GuildMember {
                characterId = operation.CharacterId,
                gameRefId = operation.GameRefId,
                guildStatus = (int)GuildMemberStatus.Owner,
                exp = operation.Exp,
                login = operation.Login,
                characterName = characterName,
                characterIcon = characterIcon
            };

            Guild newGuild = null;
            if (!application.Guilds.CreateGuild(member, operation.Name, operation.description, out newGuild)) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.Fatal, DebugMessage = "Error of creating guild" };
            }
            newGuild.name = operation.Name;

            log.InfoFormat("create guild {0}:{1} yellow", operation.Name, operation.description);
            if (!string.IsNullOrEmpty(operation.description)) {

                newGuild.description = operation.description;
            }
            application.Guilds.MarkModified(newGuild.ownerCharacterId);
            
            CreateGuildOperationResponse response = new CreateGuildOperationResponse { GuildInfo = newGuild.GetInfo(application) };
            return new OperationResponse(request.OperationCode, response);
        }
    }
}
