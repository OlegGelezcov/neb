using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class CreateGuildOperationRequest : Operation {
        public CreateGuildOperationRequest(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request) { }

        [DataMember(Code =(byte)ParameterCode.Login, IsOptional =false)]
        public string Login { get; set; }
        
        [DataMember(Code =(byte)ParameterCode.GameRefId, IsOptional =false)]
        public string GameRefId { get; set; }

        [DataMember(Code =(byte)ParameterCode.CharacterId, IsOptional =false)]
        public string CharacterId { get; set; }

        [DataMember(Code =(byte)ParameterCode.Exp, IsOptional =false)]
        public int Exp { get; set; }

        [DataMember(Code =(byte)ParameterCode.DisplayName, IsOptional =false)]
        public string Name { get; set; }

        [DataMember(Code =(byte)ParameterCode.Info, IsOptional = false)]
        public string description { get; set; }
    }
}
