using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Events {
    public enum PostTransactionAction {
        SellItemToNPC,
        PutToAuction,
        BuyAuctionItem,
        RemoveMailAttachment,
        PutItemsToAttachment,
        BuyStoreItem,
        DecreasePasses,
        IncreasePasses,
        WithdrawFromBank,
        AddToBank
    }
}
