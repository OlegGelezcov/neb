using Common;
using Photon.SocketServer.Rpc;
using System.Collections;

namespace SelectCharacter.Operations {
    public class SelectCharacterOperationResponse {

        [DataMember(Code =(byte)ParameterCode.CharacterId)]
        public string CharacterId { get; set; }

        [DataMember(Code =(byte)ParameterCode.Characters)]
        public Hashtable Characters { get; set; }
    }
}
