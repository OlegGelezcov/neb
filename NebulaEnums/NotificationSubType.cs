using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerClientCommon {
    public enum NotificationSubType : int {
        Unknown = 0,
        InviteToGroup,
        InviteToGuild,
        RequestToGuild,
        RequestFriend,
        RequestToGroup,
        MiningStationAttack,
        AuctionPurchase
    }
}
