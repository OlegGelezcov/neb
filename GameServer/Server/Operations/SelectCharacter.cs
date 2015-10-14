namespace Space.Server.Operations
{
    using Common;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class SelectCharacter : Operation
    {
        public SelectCharacter(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        { }

        /// <summary>
        /// Game ref id of user for ehich we select character
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.GameRefId)]
        public string GameRefId { get; set; }

        /// <summary>
        /// Id of character which will be selected for playing
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.CharacterId)]
        public string CharacterId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Login)]
        public string Login { get; set; }

        [DataMember(Code = (byte)ParameterCode.Password)]
        public string Password { get; set; }
    }
}
