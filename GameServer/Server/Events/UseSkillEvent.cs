using Common;
using Photon.SocketServer.Rpc;
using System.Collections;

namespace Space.Server.Events
{
    public class UseSkillEvent
    {
        [DataMember(Code = (byte)ParameterCode.Properties)]
        public Hashtable Properties { get; set; }
    }
}
