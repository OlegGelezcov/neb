using Common;
using Photon.SocketServer.Rpc;

namespace SelectCharacter.Operations {
    /// <summary>
    /// Simple response when only need return true\false values
    /// </summary>
    public class BaseSuccessResponse {
        [DataMember(Code =(byte)ParameterCode.Status)]
        public bool success { get; set; }
    }
}
