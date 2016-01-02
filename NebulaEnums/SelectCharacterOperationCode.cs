using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    /// <summary>
    /// Operation codes which valid for Select Character Node
    /// </summary>
    public enum SelectCharacterOperationCode : byte {

        GetCharacters = 0,
        DeleteCharacter = 1,
        CreateCharacter = 2,
        SelectCharacter = 3,
        GetMails = 4,
        WriteMailMessage,
        DeleteMailMessage,
        //DeleteAttachment,
        MoveAttachmentToStation,
        RegisterClient,
        GetNotifications,
        HandleNotification,
        InvokeMethod,
        CreateGuild,
        GetGuild,
        InviteToGuild,
        ExitGuild,
        DeleteGuild,
        SetGuildDescription,
        ChangeGuildMemberStatus,
        GetPlayerStore,
        BuyAuctionItem,
        DeleteAuctionItem,
        SetNewPrice,
        SendPushToPlayers,
        MoveItemFromStationToBank,
        MoveItemFromBankToStation,
        BuyPvpItem,
        RequestServerId
    }
}
