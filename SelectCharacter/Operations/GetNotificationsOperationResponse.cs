using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class GetNotificationsOperationResponse {
        [DataMember(Code =(byte)ParameterCode.Notifications, IsOptional =false)]
        public Hashtable Notifications { get; set; }
    }
}
