using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class InvokeMethodOperationResponse {
        [DataMember(Code=(byte)ParameterCode.Result)]
        public object ReturnValue { get; set; }

        [DataMember(Code =(byte)ParameterCode.Action)]
        public string MethodName { get; set; }
    }
}
