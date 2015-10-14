namespace Space.Server.Events
{
    using Common;
    using Photon.SocketServer.Rpc;
using System.Collections;

    public class WorkshopEntered
    {
        [DataMember(Code = (byte)ParameterCode.WorkshopId)]
        public byte WorkshopId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Info)]
        public Hashtable Info { get; set; }

        [DataMember(Code = (byte)ParameterCode.Type)]
        public byte Type { get; set; }
    }
}
