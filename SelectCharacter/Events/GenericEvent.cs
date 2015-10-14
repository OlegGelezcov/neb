using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Events {
    public class GenericEvent {

        [DataMember(Code =(byte)ParameterCode.SubType)]
        public int subCode { get; set; }

        [DataMember(Code =(byte)ParameterCode.EventData)]
        public Hashtable data { get; set; }
    }
}
