using Common;
using Photon.SocketServer.Rpc;

namespace SelectCharacter.Operations {
    public class RequestServerIDResponse {
        [DataMember(Code =(byte)ParameterCode.ItemId)]
        public string id { get; set; }
    }
}
