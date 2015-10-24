using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Events {
    public class PUTInventoryItemTransactionEnd : TransactionEvent{
        [DataMember(Code = (byte)ServerToServerParameterCode.GameRefId)]
        public string gameRefID { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.CharacterId)]
        public string characterID { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.ID)]
        public string itemID { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.InventoryType)]
        public byte inventoryType { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.Count)]
        public int count { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.TransactionSource)]
        public byte transactionSource { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.Success)]
        public bool success { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.Result)]
        public object result { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.ReturnCode)]
        public short returnCode { get; set; }

        public PUTInventoryItemTransactionEnd() {

        }

        public PUTInventoryItemTransactionEnd(IEventData eventData) {
            characterID = (string)eventData.Parameters[(byte)ServerToServerParameterCode.CharacterId];
            count = (int)eventData.Parameters[(byte)ServerToServerParameterCode.Count];
            gameRefID = (string)eventData.Parameters[(byte)ServerToServerParameterCode.GameRefId];
            inventoryType = (byte)eventData.Parameters[(byte)ServerToServerParameterCode.InventoryType];
            itemID = (string)eventData.Parameters[(byte)ServerToServerParameterCode.ID];
            result = eventData.Parameters[(byte)ServerToServerParameterCode.Result] as object;
            returnCode = (short)eventData.Parameters[(byte)ServerToServerParameterCode.ReturnCode];
            success = (bool)eventData.Parameters[(byte)ServerToServerParameterCode.Success];
            transactionID = eventData.Parameters[(byte)ServerToServerParameterCode.TransactionID] as string;
            transactionSource = (byte)eventData.Parameters[(byte)ServerToServerParameterCode.TransactionSource];
            if (eventData.Parameters.ContainsKey((byte)ServerToServerParameterCode.TransactionStartServer)) {
                transactionStartServer = eventData.Parameters[(byte)ServerToServerParameterCode.TransactionStartServer] as string;
            }
            if (eventData.Parameters.ContainsKey((byte)ServerToServerParameterCode.TransactionEndServer)) {
                transactionEndServer = eventData.Parameters[(byte)ServerToServerParameterCode.TransactionEndServer] as string;
            }
        }
    }
}
