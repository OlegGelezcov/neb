using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Events {
    public class NotificationUpdateEvent {
        [DataMember(Code =(byte)ParameterCode.Notifications)]
        public Hashtable Notifications { get; set; }
    }
}
