using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon {
    public enum TransactionSource : byte {
        Store,
        Mail,
        //PassManager,
        Bank,
        PvpStore
    }
}
