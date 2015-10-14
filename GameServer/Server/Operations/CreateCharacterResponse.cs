namespace Space.Server.Operations
{
    using Common;
    using Photon.SocketServer.Rpc;
    using System.Collections;

    public class CreateCharacterResponse
    {
        [DataMember(Code = (byte)ParameterCode.Status)]
        public bool Success { get; set; }

        [DataMember(Code = (byte)ParameterCode.CharacterId)]
        public string CharacterId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Info)]
        public Hashtable UserInfo { get; set; }

    }
}
