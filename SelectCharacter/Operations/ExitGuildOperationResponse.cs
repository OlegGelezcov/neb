using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class ExitGuildOperationResponse {
        [DataMember(Code =(byte)ParameterCode.Result)]
        public bool Result { get; set; }
    }
}
