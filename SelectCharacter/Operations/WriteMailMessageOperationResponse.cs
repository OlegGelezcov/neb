﻿using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class WriteMailMessageOperationResponse {
        [DataMember(Code =(byte)ParameterCode.Status)]
        public bool status { get; set; }
    }
}