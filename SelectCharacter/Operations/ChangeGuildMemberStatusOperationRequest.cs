using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class ChangeGuildMemberStatusOperationRequest : Operation {

        public ChangeGuildMemberStatusOperationRequest(IRpcProtocol protocol, OperationRequest request)
            : base (protocol, request ) { }

        [DataMember(Code =(byte)ParameterCode.GuildId, IsOptional =false)]
        public string GuildID { get; set; }

        [DataMember(Code =(byte)ParameterCode.SourceCharacterId, IsOptional =false)]
        public string SourceCharacterID { get; set; }

        [DataMember(Code =(byte)ParameterCode.CharacterId, IsOptional =false)]
        public string TargetCharacterID { get; set; }

        [DataMember(Code =(byte)ParameterCode.Status, IsOptional =false)]
        public int TargetStatus { get; set; }
    }
}
