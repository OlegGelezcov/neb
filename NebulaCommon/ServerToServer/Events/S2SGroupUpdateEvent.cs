using Photon.SocketServer.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Events {
    public class S2SGroupUpdateEvent {

        [DataMember(Code =(byte)ServerToServerParameterCode.Group)]
        public Hashtable group { get; set; }

    }
}
