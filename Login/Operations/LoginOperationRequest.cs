using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login.Operations {
    public class LoginOperationRequest : Operation{

        public LoginOperationRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) {

        }

        [DataMember(Code =(byte)ParameterCode.LoginId, IsOptional =false)]
        public string LoginId { get; set; }

        [DataMember(Code =(byte)ParameterCode.AccessToken, IsOptional =false)]
        public string AccessToken { get; set; }

    }
}
