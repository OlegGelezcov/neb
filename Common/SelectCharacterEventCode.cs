using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    public enum SelectCharacterEventCode : byte {
        NotificationUpdate,
        GuildUpdate,
        CharactersUpdate,
        ChatMessageEvent,
        GroupUpdateEvent,
        GroupRemovedEvent,
        PlayerStoreUpdate,
        MailUpdateEvent,
        GenericEvent
    }
}
