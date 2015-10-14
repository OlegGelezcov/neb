using NebulaCommon.ServerToServer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon {
    public interface IServer2ServerTransactionHandler<T, U> where T : TransactionEvent where U : TransactionEvent{
        bool HandleTransaction(T transactionStart, U transactionEnd);
    }
}
