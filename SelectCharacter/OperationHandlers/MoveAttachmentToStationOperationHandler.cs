using Common;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class MoveAttachmentToStationOperationHandler : BaseOperationHandler {
        public MoveAttachmentToStationOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            DeleteAttachmentOperationRequest operation = new DeleteAttachmentOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }
            bool result = application.Mail.StartPutAttachmentToStation(operation.Login, operation.MessageId, operation.AttachmentId);
            DeleteAttachmentOperationResponse responseObject = new DeleteAttachmentOperationResponse { Status = result };
            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
