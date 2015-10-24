using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using SelectCharacter.Operations;
using Common;

namespace SelectCharacter.OperationHandlers {
    public class WriteMessageOperationHandler : BaseOperationHandler{
        public WriteMessageOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            WriteMailMessageOperationRequest operation = new WriteMailMessageOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }
            var sourcePlayer = application.DB.GetByLogin(operation.SourceLogin);
            if (sourcePlayer == null) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.SourcePlayerNotFound,
                    DebugMessage = string.Format("player with login = {0} not found in characters database", operation.SourceLogin)
                };
            }

            var targetPlayer = application.DB.GetByLogin(operation.TargetLogin);
            if (targetPlayer == null) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.TargetPlayerNotFound,
                    DebugMessage = string.Format("target player with login = {0} not found in characters database", operation.TargetLogin)
                };
            }

            var targetMailBox = application.Mail.GetMailBox(targetPlayer.GameRefId);

            bool result = application.Mail.StartWriteMessageTransaction(sourcePlayer.GameRefId, sourcePlayer.Login, operation.inventoryType, targetPlayer.GameRefId, operation.Title, operation.Body,
                operation.Attachments, operation.targetServer);

            WriteMailMessageOperationResponse response = new WriteMailMessageOperationResponse { status = result };

            return new OperationResponse(request.OperationCode, response) {
                ReturnCode = (short)ReturnCode.Ok,
                DebugMessage = string.Empty
            };
        }
    }
}
