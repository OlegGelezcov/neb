using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    /// <summary>
    /// Describe type of history actions allowed on coalition
    /// </summary>
    public enum CoalitionTransactionType : int {
        deposit = 1,
        withdraw = 2,
        make_officier = 3,
        set_poster = 4,
        member_added = 5,
        member_removed = 6
    }
}
