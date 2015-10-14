using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class GetGuildOperationRequest : Operation {
        public GetGuildOperationRequest(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request) { }

        [DataMember(Code =(byte)ParameterCode.CharacterId)]
        public string CharacterID { get; set; }

        [DataMember(Code =(byte)ParameterCode.GameRefId)]
        public string GameRefID { get; set; }

        [DataMember(Code =(byte)ParameterCode.Page, IsOptional = true)]
        public int page { get; set; }
    }
}
