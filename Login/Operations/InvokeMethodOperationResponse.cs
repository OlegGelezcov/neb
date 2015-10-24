using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login.Operations {
    public class InvokeMethodOperationResponse {
        [DataMember(Code =(byte)ParameterCode.Result)]
        public object returnValue { get; set; }

        [DataMember(Code =(byte)ParameterCode.Action)]
        public string method { get; set; }
    }
}
