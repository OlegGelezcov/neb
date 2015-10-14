using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Events {
    public class GroupUpdateEvent {
        [DataMember(Code =(byte)ParameterCode.Group)]
        public Hashtable groupHash { get; set; }
    }
}
