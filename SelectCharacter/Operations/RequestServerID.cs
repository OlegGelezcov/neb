using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace SelectCharacter.Operations {
    public class RequestServerID : Operation {
        public RequestServerID(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request) { }
    }
}
