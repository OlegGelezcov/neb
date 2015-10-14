namespace Space.Server.Operations
{
    using System.Collections;
    using Common;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Space.Mmo.Server;

    public class EnterWorkshop : Operation
    {
        public EnterWorkshop(IRpcProtocol protocol, OperationRequest request ) 
            : base(protocol, request )
        {

        }

        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Type)]
        public byte Type { get; set; }
    }
}
