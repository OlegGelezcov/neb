using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Login.Operations {
    public class LoginOperationRequest : Operation{

        public LoginOperationRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) {

        }

        [DataMember(Code =(byte)ParameterCode.Login, IsOptional =false)]
        public string login { get; set; }

        [DataMember(Code =(byte)ParameterCode.Password, IsOptional =false)]
        public string encryptedPassword { get; set; }

    }
}
