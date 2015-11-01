﻿using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace SelectCharacter.Operations {
    public class MoveItemFromStationToBankRequest : Operation {
        public MoveItemFromStationToBankRequest(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request) { }

        [DataMember(Code = (byte)ParameterCode.ItemId, IsOptional = false)]
        public string item { get; set; }

        [DataMember(Code =(byte)ParameterCode.Count, IsOptional = false)]
        public int count { get; set; }

        [DataMember(Code =(byte)ParameterCode.ServerId, IsOptional = false)]
        public string server { get; set; }
    }
}