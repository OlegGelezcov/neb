using Common;
using Photon.SocketServer.Rpc;

namespace Login.Events {
    public class InapStatusUpdateEvent {

        [DataMember(Code = (byte)ParameterCode.Status)]
        public bool success { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string inapId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Price)]
        public int price { get; set; }
    }
}
