using NebulaCommon;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System.Collections;

namespace NebulaCommon.ServerToServer.Operations {
    public class RegisterGameServerResponse : DataContract {

        public RegisterGameServerResponse(IRpcProtocol protocol, OperationResponse response)
            : base(protocol, response.Parameters) {

        }

        public RegisterGameServerResponse() {

        }

        [DataMember(Code =(byte)ServerToServerParameterCode.AuthList, IsOptional =true)]
        public Hashtable AuthList { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.ExternalAddress, IsOptional =true)]
        public byte[] ExternalAddress { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.InternalAddress)]
        public byte[] InternalAddress { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.SharedKey, IsOptional =true)]
        public byte[] SharedKey { get; set; }
    }
}
