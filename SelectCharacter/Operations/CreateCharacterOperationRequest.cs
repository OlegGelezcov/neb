using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace SelectCharacter.Operations {

    /// <summary>
    /// Request contract for creating new character
    /// </summary>
    public class CreateCharacterOperationRequest : Operation {

        public CreateCharacterOperationRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) {

        }

        [DataMember(Code =(byte)ParameterCode.GameRefId, IsOptional =false)]
        public string GameRefId { get; set; }

        [DataMember(Code =(byte)ParameterCode.Race, IsOptional =false)]
        public byte Race { get; set; }

        [DataMember(Code =(byte)ParameterCode.WorkshopId, IsOptional =false)]
        public byte Workshop { get; set; }

        [DataMember(Code =(byte)ParameterCode.DisplayName, IsOptional =false)]
        public string DisplayName { get; set; }

        [DataMember(Code = (byte)ParameterCode.Icon, IsOptional = false )]
        public int icon { get; set; }
    }
}
