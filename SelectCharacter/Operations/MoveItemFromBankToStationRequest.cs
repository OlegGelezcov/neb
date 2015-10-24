using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class MoveItemFromBankToStationRequest : Operation {

        public MoveItemFromBankToStationRequest(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request) {

        }

        [DataMember(Code =(byte)ParameterCode.ItemId, IsOptional = false)]
        public string item { get; set; }

        [DataMember(Code =(byte)ParameterCode.Count, IsOptional = false)]
        public int count { get; set; }

        [DataMember(Code =(byte)ParameterCode.ServerId, IsOptional = false)]
        public string server { get; set; }
    }
}
