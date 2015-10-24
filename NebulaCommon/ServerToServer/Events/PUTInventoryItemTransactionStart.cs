using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Events {
    public class PUTInventoryItemTransactionStart : TransactionEvent {

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

        [DataMember(Code = (byte)ServerToServerParameterCode.Action)]
        public byte postTransactionAction { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.Tag)]
        public object tag { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.TargetObject)]
        public object targetObject { get; set; }

        public PUTInventoryItemTransactionStart() {

        }

        public PUTInventoryItemTransactionStart(IEventData eventData) {
            characterID = eventData.Parameters[(byte)ServerToServerParameterCode.CharacterId] as string;
            count = (int)eventData.Parameters[(byte)ServerToServerParameterCode.Count];
            gameRefID = eventData.Parameters[(byte)ServerToServerParameterCode.GameRefId] as string;
            inventoryType = (byte)eventData.Parameters[(byte)ServerToServerParameterCode.InventoryType];
            itemID = eventData.Parameters[(byte)ServerToServerParameterCode.ID] as string;
            postTransactionAction = (byte)eventData.Parameters[(byte)ServerToServerParameterCode.Action];
            tag = eventData.Parameters[(byte)ServerToServerParameterCode.Tag];
            targetObject = eventData.Parameters[(byte)ServerToServerParameterCode.TargetObject];
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
