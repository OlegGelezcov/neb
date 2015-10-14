using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Master.Operations {
    public class GetServerVersionResponse {
        [DataMember(Code =(byte)ParameterCode.Info, IsOptional =false)]
        public string serverVersion { get; set; }
    }
}
