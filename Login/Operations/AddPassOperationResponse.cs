using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login.Operations {
    public class AddPassOperationResponse {
        [DataMember(Code =(byte)ParameterCode.Status)]
        public int returnCode { get; set; }
    }
}
