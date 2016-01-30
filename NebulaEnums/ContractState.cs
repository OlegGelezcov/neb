using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    public enum ContractState : int {
        accepted = 1,
        ready = 2,
        completed = 3,
        unknown = 4
    }
}
