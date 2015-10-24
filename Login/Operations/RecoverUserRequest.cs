using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login.Operations {
    public class RecoverUserRequest : Operation {
        public RecoverUserRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) { }

        [DataMember(Code =(byte)ParameterCode.Email, IsOptional =false)]
        public string email { get; set; }

        [DataMember(Code =(byte)ParameterCode.Language, IsOptional = false)]
        public string language { get; set; }
    }
}
