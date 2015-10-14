using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class GetNotificationsOperationRequest : Operation {
        public GetNotificationsOperationRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code =(byte)ParameterCode.CharacterId, IsOptional =false)]
        public string CharacterID { get; set; }
    }
}
