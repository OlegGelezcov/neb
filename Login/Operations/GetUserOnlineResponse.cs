using Common;
using Photon.SocketServer.Rpc;

namespace Login.Operations {
    public class GetUserOnlineResponse {
        [DataMember(Code =(byte)ParameterCode.Count, IsOptional =false)]
        public int count { get; set; }
    }
}
