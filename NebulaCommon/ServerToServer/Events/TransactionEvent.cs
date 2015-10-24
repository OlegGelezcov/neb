using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Events {
    public abstract class TransactionEvent {
        [DataMember(Code = (byte)ServerToServerParameterCode.TransactionID)]
        public string transactionID { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.TransactionStartServer, IsOptional = true)]
        public string transactionStartServer { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.TransactionEndServer, IsOptional = true)]
        public string transactionEndServer { get; set; }

    }
}
