using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class DeleteMailMessageRequest : Operation {

        public DeleteMailMessageRequest(IRpcProtocol protocol, OperationRequest operationRequest ) : base(protocol, operationRequest) { }

        [DataMember(Code =(byte)ParameterCode.MessageId, IsOptional =false)]
        public string MessageId { get; set; }

        [DataMember(Code =(byte)ParameterCode.SourceLogin, IsOptional =false)]
        public string Login { get; set; }

    }
}
