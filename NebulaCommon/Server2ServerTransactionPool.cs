using NebulaCommon.ServerToServer.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon {
    public class Server2ServerTransactionPool<T, U> 

        where T : TransactionEvent
        where U : TransactionEvent {

        private  ConcurrentDictionary<string, T> startedTransactions { get; set; }
        private IServer2ServerTransactionHandler<T, U> mHandler;

        public Server2ServerTransactionPool(IServer2ServerTransactionHandler<T, U> handler ) {
            startedTransactions = new ConcurrentDictionary<string, T>();
            mHandler = handler;
        }


        public bool StartTransaction(T transactionStart) {
            if(!startedTransactions.ContainsKey(transactionStart.transactionID)) {
                return startedTransactions.TryAdd(transactionStart.transactionID, transactionStart);
            }
            return false;
        }

        public bool HandleTransaction(U transactionEnd) {
            if(startedTransactions.ContainsKey(transactionEnd.transactionID)) {
                T value;
                if ( startedTransactions.TryRemove(transactionEnd.transactionID, out value ) ) {
                    if(mHandler != null ) {
                        return mHandler.HandleTransaction(value, transactionEnd);
                    }
                }
            }
            return false;
        }
    }
}
