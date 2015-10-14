using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Events {
    public class GroupRemovedEvent {
        [DataMember(Code=(byte)ParameterCode.Group)]
        public string Group { get; set; }
    }
}
