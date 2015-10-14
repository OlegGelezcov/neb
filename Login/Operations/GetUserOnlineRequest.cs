using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Login.Operations {
    public class GetUserOnlineRequest : Operation {
        public GetUserOnlineRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) { }
        public GetUserOnlineRequest() { }
    }
}
