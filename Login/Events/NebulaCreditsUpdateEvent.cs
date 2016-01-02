using Common;
using Photon.SocketServer.Rpc;

namespace Login.Events {
    public class NebulaCreditsUpdateEvent {
        [DataMember(Code = (byte)ParameterCode.Count )]
        public int nebulaCredits { get; set; }
    }
}
