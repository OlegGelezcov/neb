using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SelectCharacter.Operations {

    //Characters response
    class GetCharactersOperationResponse {

        [DataMember(Code =(byte)ParameterCode.Characters)]
        public Hashtable Characters { get; set; }

    }
}
