using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class SendPushToPlayersOperationRequest : Operation {
        public SendPushToPlayersOperationRequest(IRpcProtocol protocol, OperationRequest request): base(protocol, request) { }

        [DataMember(Code =(byte)ParameterCode.Type)]
        public byte pushType { get; set; }

        [DataMember(Code =(byte)ParameterCode.Body)]
        public string body { get; set; }

        [DataMember(Code =(byte)ParameterCode.Title)]
        public string title { get; set; }
    }
}
