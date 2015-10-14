using Common;
using Photon.SocketServer.Rpc;
using System.Collections;

namespace SelectCharacter.Events {
    public class MailUpdatedEvent {
        [DataMember(Code =(byte)ParameterCode.MailBox)]
        public Hashtable mailBox { get; set; }
    }
}
