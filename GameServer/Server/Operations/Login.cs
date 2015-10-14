
namespace Space.Server.Operations
{
    using Common;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class Login : Operation
    {
        public Login(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { 
            
        }

        [DataMember(Code = (byte)ParameterCode.Login)]
        public string LoginName { get; set; }

        [DataMember(Code = (byte)ParameterCode.Password)]
        public string Password { get; set; }

        [DataMember(Code = (byte)ParameterCode.Username)]
        public string DisplayName { get; set; }

    }


}
