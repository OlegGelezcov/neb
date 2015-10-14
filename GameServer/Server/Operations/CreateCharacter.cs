namespace Space.Server.Operations
{
    using Common;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class CreateCharacter : Operation
    {
        public CreateCharacter(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        { }

        [DataMember(Code = (byte)ParameterCode.WorkshopId)]
        public byte Workshop { get; set; }

        [DataMember(Code=(byte)ParameterCode.Race)]
        public byte Race {get;set;}

        [DataMember(Code = (byte)ParameterCode.GameRefId)]
        public string GameRefId { get; set; }

    }
}
