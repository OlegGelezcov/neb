using Photon.SocketServer;
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

        public GETInventoryItemsTransactionEnd() {

        }

        public GETInventoryItemsTransactionEnd(IEventData eventData) {
            characterID = (string)eventData[(byte)ServerToServerParameterCode.CharacterId];
            gameRefID = (string)eventData[(byte)ServerToServerParameterCode.GameRefId];
            inventoryType = (byte)eventData[(byte)ServerToServerParameterCode.InventoryType];
            items = (Hashtable)eventData[(byte)ServerToServerParameterCode.Items];
            result = eventData[(byte)ServerToServerParameterCode.Result];
            returnCode = (short)eventData[(byte)ServerToServerParameterCode.ReturnCode];
            success = (bool)eventData[(byte)ServerToServerParameterCode.Success];
            transactionID = (string)eventData[(byte)ServerToServerParameterCode.TransactionID];
            transactionSource = (byte)eventData[(byte)ServerToServerParameterCode.TransactionSource];
            if (eventData.Parameters.ContainsKey((byte)ServerToServerParameterCode.TransactionStartServer)) {
                transactionStartServer = eventData.Parameters[(byte)ServerToServerParameterCode.TransactionStartServer] as string;
            }
            if (eventData.Parameters.ContainsKey((byte)ServerToServerParameterCode.TransactionEndServer)) {
                transactionEndServer = eventData.Parameters[(byte)ServerToServerParameterCode.TransactionEndServer] as string;
            }
        }
    }
}
