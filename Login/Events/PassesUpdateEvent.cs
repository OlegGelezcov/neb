﻿using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login.Events {
    public class PassesUpdateEvent {
        [DataMember(Code =(byte)ParameterCode.Passes) ]
        public int passes { get; set; }

        [DataMember(Code =(byte)ParameterCode.ExpireTime)]
        public int expireTime { get; set; }

        [DataMember(Code = (byte)ParameterCode.CurrentTime)]
        public int currentTime { get; set; }
    }
}
