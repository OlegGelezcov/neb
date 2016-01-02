using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class RequestServerIDOperationHandler : BaseOperationHandler  {

        public RequestServerIDOperationHandler(SelectCharacterApplication context, SelectCharacterClientPeer peer)
            : base(context, peer ) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            var operation = new RequestServerID(peer.Protocol, request);
            RequestServerIDResponse responseData = new RequestServerIDResponse {
                id = SelectCharacterApplication.ServerId.ToString()
            };
            return new OperationResponse(request.OperationCode, responseData);
        }
    }
}
