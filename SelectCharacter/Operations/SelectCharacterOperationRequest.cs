using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace SelectCharacter.Operations {
    public class SelectCharacterOperationRequest : Operation {

        public SelectCharacterOperationRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) { }

        [DataMember(Code =(byte)ParameterCode.GameRefId, IsOptional =false)]
        public string GameRefId { get; set; }

        [DataMember(Code =(byte)ParameterCode.CharacterId, IsOptional =false)]
        public string CharacterId { get; set; }
    }
}
