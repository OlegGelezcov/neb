using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class HandleNotificationOperationRequest : Operation {

        public HandleNotificationOperationRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code =(byte)ParameterCode.NotificationId, IsOptional =false)]
        public string NotificationID { get; set; }

        [DataMember(Code =(byte)ParameterCode.CharacterId, IsOptional =false)]
        public string CharacterID { get; set; }

        [DataMember(Code =(byte)ParameterCode.Result, IsOptional =false)]
        public bool Respond { get; set; }

    }
}
