using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class InviteGuildOperationRequest : Operation {

        public InviteGuildOperationRequest(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request) { }

        [DataMember(Code = (byte)ParameterCode.SourceLogin, IsOptional = false)]
        public string SourceLogin { get; set; }

        [DataMember(Code =(byte)ParameterCode.SourceCharacterId, IsOptional =false)]
        public string SourceCharacterId { get; set; }


        [DataMember(Code =(byte)ParameterCode.TargetLogin, IsOptional =false)]
        public string TargetLogin { get; set; }

        [DataMember(Code =(byte)ParameterCode.CharacterId, IsOptional =false)]
        public string TargetCharacterId { get; set; }

        [DataMember(Code =(byte)ParameterCode.GuildId, IsOptional =false)]
        public string GuildID { get; set; }

    }
}
