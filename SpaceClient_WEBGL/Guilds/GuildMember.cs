using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Guilds {
    public class GuildMember : IInfoParser {
        public string login { get; private set; }
        public string gameRefID { get; private set; }
        public string characterID { get; private set; }
        public int guildStatus { get; private set; }
        public int exp { get; private set; }

        public GuildMember(Hashtable info) { ParseInfo(info); }

        public void ParseInfo(Hashtable info) {
            login = info.GetValueString((int)SPC.Login);
            gameRefID = info.GetValueString((int)SPC.GameRefId);
            characterID = info.GetValueString((int)SPC.CharacterId);
            guildStatus = info.GetValueInt((int)SPC.Status);
            exp = info.GetValueInt((int)SPC.Exp);
        }

        public bool GrantedAddMember() {
            return (guildStatus == (int)GuildMemberStatus.Moderator) || (guildStatus == (int)GuildMemberStatus.Owner);
        }

        public bool GrantedSetDescription() {
            return (guildStatus == (int)GuildMemberStatus.Moderator) || (guildStatus == (int)GuildMemberStatus.Owner);
        }

        public bool GrantedChangeStatusFromTo(GuildMemberStatus fromStatus, GuildMemberStatus toStatus) {
            GuildMemberStatus myStatus = (GuildMemberStatus)guildStatus;
            if (myStatus == GuildMemberStatus.Moderator) {
                if (fromStatus == GuildMemberStatus.Member && toStatus == GuildMemberStatus.Moderator) {
                    return true;
                }
            }
            if (myStatus == GuildMemberStatus.Owner) {
                if (fromStatus != GuildMemberStatus.Owner) {
                    if (fromStatus != toStatus) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
