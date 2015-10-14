using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace SelectCharacter.Operations {
    public class DeleteGuildOperationRequest : Operation {

        public DeleteGuildOperationRequest(IRpcProtocol protocol, OperationRequest request): base(protocol, request) { }

        [DataMember(Code =(byte)ParameterCode.SourceCharacterId)]
        public string SourceCharacterID { get; set; }
    }
}
