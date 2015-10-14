using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class DeleteAttachmentOperationResponse {

        [DataMember(Code =(byte)ParameterCode.Status)]
        public bool Status { get; set; }
    }
}
