using Common;
using Photon.SocketServer.Rpc;
using System.Collections;

namespace Master.Operations {
    public class GetNewsResponse {

        [DataMember(Code =(byte)ParameterCode.Info)]
        public Hashtable news { get; set; }
    }
}
