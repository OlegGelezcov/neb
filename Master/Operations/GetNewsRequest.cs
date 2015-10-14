using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Master.Operations {
    public class GetNewsRequest : Operation {

        public GetNewsRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) { }

        [DataMember(Code =(byte)ParameterCode.Type, IsOptional =false)]
        public string lang { get; set; }
    }
}
