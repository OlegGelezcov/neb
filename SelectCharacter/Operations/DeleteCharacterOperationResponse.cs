using Common;
using Photon.SocketServer.Rpc;
using System.Collections;

namespace SelectCharacter.Operations {
    public class DeleteCharacterOperationResponse {

        [DataMember(Code =(byte)ParameterCode.Characters, IsOptional =false)]
        public Hashtable Characters { get; set; }
    }

}
