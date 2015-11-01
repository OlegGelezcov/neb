﻿using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login.Operations {
    public class AddPassRequest : Operation{
        public AddPassRequest(IRpcProtocol protocol, OperationRequest operationRequest ) 
            : base(protocol, operationRequest) { }

        [DataMember(Code =(byte)ParameterCode.Login, IsOptional = false)]
        public string login { get; set; }

        [DataMember(Code =(byte)ParameterCode.GameRefId, IsOptional = false)]
        public string gameRef { get; set; }
    }
}