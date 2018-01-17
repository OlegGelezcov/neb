using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;

namespace Login.Operations {
    public class LoginOperationRequest : Operation{

        public LoginOperationRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) {

        }

        [DataMember(Code =(byte)ParameterCode.Login, IsOptional =false)]
        public string login { get; set; }

        [DataMember(Code =(byte)ParameterCode.Password, IsOptional =false)]
        public string encryptedPassword { get; set; }

        [DataMember(Code =(byte)ParameterCode.FacebookId, IsOptional = false)]
        public string facebookId { get; private set; }

        [DataMember(Code = (byte)ParameterCode.VkontakteId, IsOptional = false)]
        public string vkontakteId { get; private set; }

        [DataMember(Code =(byte)ParameterCode.DeviceId, IsOptional = true)]
        public string deviceId { get; private set; }

        [DataMember(Code = (byte)ParameterCode.Method, IsOptional = false)]
        public byte method { get; private set; }

        [DataMember(Code = (byte)ParameterCode.Platform, IsOptional = true )]
        public string platform { get; private set; }

        public bool TryGetLoginMethod(out LoginMethod loginMethod) {
            loginMethod = LoginMethod.device_id;
            try {
                loginMethod = (LoginMethod)method;
                return true;
            } catch(Exception) {
                return false;
            }
        }
    }
}
