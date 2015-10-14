using Common;
using Photon.SocketServer.Rpc;
using System.Collections;

namespace SelectCharacter.Operations {
    public class HandleNotificationOperationResponse {
        [DataMember(Code =(byte)ParameterCode.Notifications)]
        public Hashtable Notifications { get; set; }
    }
}
