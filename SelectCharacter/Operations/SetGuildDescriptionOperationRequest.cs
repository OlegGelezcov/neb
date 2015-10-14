using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class SetGuildDescriptionOperationRequest : Operation{

        public SetGuildDescriptionOperationRequest(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request) { }

        [DataMember(Code =(byte)ParameterCode.CharacterId, IsOptional =false)]
        public string CharacterID { get; set; }

        [DataMember(Code =(byte)ParameterCode.GuildId, IsOptional =false)]
        public string GuildID { get; set; }

        [DataMember(Code =(byte)ParameterCode.Info, IsOptional =false)]
        public string Description { get; set; }
    }
}
