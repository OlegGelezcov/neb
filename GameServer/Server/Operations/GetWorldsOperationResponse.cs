using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Server.Operations {
    public class GetWorldsOperationResponse {

        [DataMember(Code =(byte)ParameterCode.Worlds)]
        public Hashtable worlds { get; set; }

    }
}
