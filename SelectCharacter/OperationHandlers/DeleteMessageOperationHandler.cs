using Common;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class DeleteMessageOperationHandler : BaseOperationHandler {
        public DeleteMessageOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            DeleteMailMessageRequest operation = new DeleteMailMessageRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }
            var sourcePlayer = application.DB.GetByLogin(operation.Login);
            if (sourcePlayer == null) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.SourcePlayerNotFound,
                    DebugMessage = string.Format("player with login = {0} not founded", operation.Login)
                };
            }

            var sourceMailBox = application.Mail.GetMailBox(sourcePlayer.GameRefId);
            if (!sourceMailBox.DeleteMessage(operation.MessageId)) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.InvalidOperation,
                    DebugMessage = string.Format("message with id = {0} not founded", operation.MessageId)
                };
            }

            application.Mail.SaveMails(sourceMailBox);

            return new OperationResponse(request.OperationCode) {
                ReturnCode = (short)ReturnCode.Ok,
                DebugMessage = string.Empty
            };
        }
    }
}
