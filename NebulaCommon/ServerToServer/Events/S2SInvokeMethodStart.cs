using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Events {
    public class S2SInvokeMethodStart {
        [DataMember(Code =(byte)ServerToServerParameterCode.Method)]
        public string method { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.Arguments)]
        public object[] arguments { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.SourceServer)]
        public string sourceServerID { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.TargetServer)]
        public byte targetServerType { get; set; }
    }
}
