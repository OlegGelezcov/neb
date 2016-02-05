using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Nebula.Server.Operations {

    public class RPCInvokeOperation : Operation {
        public RPCInvokeOperation(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request) { }

        [DataMember(Code =(byte)ParameterCode.Action)]
        public int rpcId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Parameters)]
        public object[] parameters { get; set; }
    }

    public class RPCInvokeResponse {
        [DataMember(Code = (byte)ParameterCode.Action)]
        public int rpcId { get; set; }

        [DataMember(Code =(byte)ParameterCode.Result)]
        public object result { get; set; }
    }
}
