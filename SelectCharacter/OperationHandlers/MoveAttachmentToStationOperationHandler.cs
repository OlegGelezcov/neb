using Common;
using ExitGames.Logging;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class MoveAttachmentToStationOperationHandler : BaseOperationHandler {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public MoveAttachmentToStationOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            DeleteAttachmentOperationRequest operation = new DeleteAttachmentOperationRequest(peer.Protocol, request);
            log.InfoFormat("MoveAttachmentToStationOperation = {0} [red]", operation.ToString());
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse(operation);
            }
            bool result = application.Mail.StartPutAttachmentToStation(operation.Login, operation.MessageId, operation.AttachmentId, operation.targetServer);
            DeleteAttachmentOperationResponse responseObject = new DeleteAttachmentOperationResponse { Status = result };
            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
