using Common;
using Photon.SocketServer.Rpc;

namespace SelectCharacter.Operations {
    public class DeleteGuildOperationResponse {
        [DataMember(Code=(byte)ParameterCode.Result)]
        public bool Result { get; set; }
    }
}
