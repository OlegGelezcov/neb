using Common;
using Photon.SocketServer.Rpc;
using System.Collections;

namespace SelectCharacter.Operations {
    public class GetMailBoxOperationResponse {

        [DataMember(Code =(byte)ParameterCode.MailBox)]
        public Hashtable Mails { get; set; }
    }
}
