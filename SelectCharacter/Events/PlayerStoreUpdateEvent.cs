using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Events {
    public class PlayerStoreUpdateEvent {
        [DataMember(Code =(byte)ParameterCode.Info)]
        public Hashtable storeInfo { get; set; }
    }
}
