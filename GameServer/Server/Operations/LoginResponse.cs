namespace Space.Server.Operations
{
    using Common;
    using Photon.SocketServer.Rpc;
    using System.Collections;

    public class LoginResponse
    {
        [DataMember(Code = (byte)ParameterCode.Status)]
        public bool Success { get; set; }

        [DataMember(Code = (byte)ParameterCode.GameRefId)]
        public string GameRefId { get; set; }

        [DataMember(Code=(byte)ParameterCode.Username)]
        public string DisplayName {get;set;}

        [DataMember(Code = (byte)ParameterCode.Info)]
        public Hashtable UserInfo { get; set; }

        [DataMember(Code = (byte)ParameterCode.Login)]
        public string LoginName { get; set; }

        [DataMember(Code = (byte)ParameterCode.Password)]
        public string Password { get; set; }
    }
}
