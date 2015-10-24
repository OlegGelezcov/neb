using Common;
using Photon.SocketServer.Rpc;

namespace Nebula.Server.Operations {
    public class RequestServerIDResponse {
        [DataMember(Code =(byte)ParameterCode.ItemId)]
        public string id { get; set; }
    }
}
