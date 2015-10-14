using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using SelectCharacter.Operations;
using Common;

namespace SelectCharacter.OperationHandlers {
    public class ChangeGuildMemberStatusOperationHandler : BaseOperationHandler {

        public ChangeGuildMemberStatusOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            ChangeGuildMemberStatusOperationRequest operation = new ChangeGuildMemberStatusOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }
            bool success = application.Guilds.ChangeGuildMemberStatus(operation.SourceCharacterID, operation.TargetCharacterID, operation.GuildID, operation.TargetStatus);
            if (!success) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.LogicError, DebugMessage = "Error of changing status" };
            } else {
                return new OperationResponse(request.OperationCode);
            }
        }
    }
}
