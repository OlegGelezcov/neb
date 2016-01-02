using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Login.Operations {
    public class GetNebulaCreditsRequest : Operation {
        public GetNebulaCreditsRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) { }

        [DataMember(Code =(byte)ParameterCode.GameRefId, IsOptional = false )]
        public string gameRef { get; set; }
    }
}
