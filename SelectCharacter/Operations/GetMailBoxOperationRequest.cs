using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace SelectCharacter.Operations {
    public class GetMailBoxOperationRequest : Operation {

        public GetMailBoxOperationRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) { }

        [DataMember(Code =(byte)ParameterCode.GameRefId, IsOptional =false)]
        public string GameRefId { get; set; }
    }
}
