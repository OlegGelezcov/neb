using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class InvokeMethodOperationRequest  : Operation {
        public InvokeMethodOperationRequest(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request) { }

        [DataMember(Code =(byte)ParameterCode.Action, IsOptional =false)]
        public string MethodName { get; set; }

        [DataMember(Code =(byte)ParameterCode.Arguments, IsOptional =true)]
        public object[] Arguments { get; set; }
    }
}
