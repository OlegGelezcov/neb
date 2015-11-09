using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    public enum SelectCharacterGenericEventSubCode : int {
        ReceiveCredits,
        CommanderElected,
        FriendsUpdate,
        CharactersUpdate,
        ConsumablePurchaseStatus,
        NewMessageCountChanged,
        CommandsUpdate,
        Push,
        BankUpdate,
        PvpPointUpdate,
        PvpStorePurchaseStatus
    }
}
