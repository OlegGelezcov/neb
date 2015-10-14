using Photon.SocketServer.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Events {
    public class GETInventoryItemsTransactionEnd : TransactionEvent {
        [DataMember(Code = (byte)ServerToServerParameterCode.GameRefId)]
        public string gameRefID { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.CharacterId)]
        public string characterID { get; set; }


        [DataMember(Code = (byte)ServerToServerParameterCode.InventoryType)]
        public byte inventoryType { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.Items)]
        public Hashtable items { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.TransactionSource)]
        public byte transactionSource { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.Success)]
        public bool success { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.Result)]
        public object result { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.ReturnCode)]
        public short returnCode { get; set; }
    }
}
