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
    }
}
