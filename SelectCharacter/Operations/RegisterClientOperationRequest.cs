using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class RegisterClientOperationRequest : Operation {

        public RegisterClientOperationRequest() { }

        public RegisterClientOperationRequest(IRpcProtocol rpcProtocol, OperationRequest operationRequest)
            : base(rpcProtocol, operationRequest) { }

        [DataMember(Code =(byte)ParameterCode.GameRefId)]
        public string GameRefId { get; set; }

    }
}
