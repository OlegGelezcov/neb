using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class GetPlayerStoreOperationResponse {

        [DataMember(Code =(byte)ParameterCode.Info)]
        public Hashtable info { get; set; }
    }
}
