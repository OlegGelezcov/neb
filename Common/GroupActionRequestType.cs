using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    public enum GroupActionRequestType : byte {
        InviteToNewGroup,
        InviteToExistingGroup,
        ExcludePlayerFromGroup,
        ExitFromGroup,
        None
    }
}
