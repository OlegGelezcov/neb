using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client.Guilds {
    public class GuildMember : IInfoParser {
        public string login { get; private set; }
        public string gameRefID { get; private set; }
        public string characterID { get; private set; }
        public int guildStatus { get; private set; }
        public int exp { get; private set; }

        public GuildMember(Hashtable info) { ParseInfo(info);  }

        public void ParseInfo(Hashtable info) {
            login           = info.Value<string>((int)SPC.Login,        string.Empty);
            gameRefID       = info.Value<string>((int)SPC.GameRefId,    string.Empty );
            characterID     = info.Value<string>((int)SPC.CharacterId,  string.Empty);
            guildStatus     = info.Value<int>((int)SPC.Status);
            exp             = info.Value<int>((int)SPC.Exp);
        }

        public bool GrantedAddMember() {
            return (guildStatus == (int)GuildMemberStatus.Moderator) || (guildStatus == (int)GuildMemberStatus.Owner);
        }

        public bool GrantedSetDescription() {
            return (guildStatus == (int)GuildMemberStatus.Moderator) || (guildStatus == (int)GuildMemberStatus.Owner);
        }
        
        public bool GrantedChangeStatusFromTo(GuildMemberStatus fromStatus, GuildMemberStatus toStatus) {
            GuildMemberStatus myStatus = (GuildMemberStatus)guildStatus;
            if(myStatus == GuildMemberStatus.Moderator ) {
                if(fromStatus == GuildMemberStatus.Member && toStatus == GuildMemberStatus.Moderator) {
                    return true;
                }
            }
            if(myStatus == GuildMemberStatus.Owner) {
                if(fromStatus != GuildMemberStatus.Owner) {
                    if(fromStatus != toStatus) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
