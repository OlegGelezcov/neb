
namespace Space.Server.Operations
{
    using Common;
    using Photon.SocketServer.Rpc;
    using System.Collections;

    public class EnterWorkshopResponse
    {
        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }


        [DataMember(Code = (byte)ParameterCode.Info)]
        public Hashtable Info { get; set; }

        [DataMember(Code=(byte)ParameterCode.Type)]
        public byte Type {get;set;}
    }
}
