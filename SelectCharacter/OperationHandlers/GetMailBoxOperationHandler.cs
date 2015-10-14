using Common;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class GetMailBoxOperationHandler : BaseOperationHandler {
        public GetMailBoxOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            GetMailBoxOperationRequest operation = new GetMailBoxOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            var mails = application.Mail.GetMails(operation.GameRefId);
            GetMailBoxOperationResponse responseObject = new GetMailBoxOperationResponse { Mails = mails };
            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
