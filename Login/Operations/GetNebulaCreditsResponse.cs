using Common;
using Photon.SocketServer.Rpc;

namespace Login.Operations {
    public class GetNebulaCreditsResponse {
        [DataMember(Code =(byte)ParameterCode.Count)]
        public int count { get; set; }
    }
}
