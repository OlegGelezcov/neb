using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Events {
    public class CharacterUpdateEvent {
        [DataMember(Code =(byte)ParameterCode.Characters)]
        public Hashtable Characters { get; set; }
    }
}
