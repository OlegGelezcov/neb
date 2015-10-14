using Common;
using Photon.SocketServer.Rpc;

namespace SelectCharacter.Operations {
    public class SetNewPriceOperationResponse {
        [DataMember(Code =(byte)ParameterCode.Status)]
        public bool status { get; set; }

    }
}
