using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Events {
    public class S2SInvokeMethodEnd {
        [DataMember(Code = (byte)ServerToServerParameterCode.SourceServer)]
        public string sourceServerID { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.TargetServer)]
        public byte targetServerType { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.Method)]
        public string method { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.Result)]
        public  object result { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.Success)]
        public bool callSuccess { get; set; }
    }
}
