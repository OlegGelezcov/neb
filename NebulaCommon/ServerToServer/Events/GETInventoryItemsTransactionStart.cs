using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Events {
    public class GETInventoryItemsTransactionStart : TransactionEvent {
        [DataMember(Code = (byte)ServerToServerParameterCode.GameRefId)]
        public string gameRefID { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.CharacterId)]
        public string characterID { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.InventoryType)]
        public byte inventoryType { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.Items)]
        public Hashtable items { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.TransactionSource)]
        public byte transactionSource { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.Action)]
        public byte postTransactionAction { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.Tag)]
        public object tag { get; set; }

        private object notSended;

        public object GetNotSended() {
            return notSended;
        }

        public void SetNotSended(object o) {
            notSended = o;
        }

        public GETInventoryItemsTransactionStart() {

        }

        public GETInventoryItemsTransactionStart(IEventData eventData) {
            characterID = (string)eventData[(byte)ServerToServerParameterCode.CharacterId];
            gameRefID = (string)eventData[(byte)ServerToServerParameterCode.GameRefId];
            inventoryType = (byte)eventData[(byte)ServerToServerParameterCode.InventoryType];
            transactionID = (string)eventData[(byte)ServerToServerParameterCode.TransactionID];
            transactionSource = (byte)eventData[(byte)ServerToServerParameterCode.TransactionSource];
            postTransactionAction = (byte)eventData[(byte)ServerToServerParameterCode.Action];
            tag = eventData.Parameters[(byte)ServerToServerParameterCode.Tag] as object;
            items = eventData.Parameters[(byte)ServerToServerParameterCode.Items] as Hashtable;
            if (eventData.Parameters.ContainsKey((byte)ServerToServerParameterCode.TransactionStartServer)) {
                transactionStartServer = eventData.Parameters[(byte)ServerToServerParameterCode.TransactionStartServer] as string;
            }
            if (eventData.Parameters.ContainsKey((byte)ServerToServerParameterCode.TransactionEndServer)) {
                transactionEndServer = eventData.Parameters[(byte)ServerToServerParameterCode.TransactionEndServer] as string;
            }
        }
    }
}
