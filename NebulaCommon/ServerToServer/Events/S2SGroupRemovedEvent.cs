using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Events {
    public class S2SGroupRemovedEvent {
        [DataMember(Code =(byte)ServerToServerParameterCode.Group)]
        public string Group { get; set; }
    }
}
