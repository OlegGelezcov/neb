using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace SelectCharacter.Operations {

    /// <summary>
    /// Get characters operation request
    /// </summary>
    class GetCharactersOperationRequest : Operation {

        public GetCharactersOperationRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) {

        }

        [DataMember(Code =(byte)ParameterCode.GameRefId, IsOptional =false)]
        public string GameRefId { get; set; }

        [DataMember(Code =(byte)ParameterCode.Login, IsOptional =false)]
        public string Login { get; set; }
    }
}
