
namespace Space.Server.Events
{
    using Common;
    using Photon.SocketServer.Rpc;

    public class WorkshopExited
    {
        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Position)]
        public float[] Position { get; set; }
    }
}
