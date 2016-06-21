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
        public string characterName { get; private set; } = string.Empty;
        public int characterIcon { get; private set; } = -1;
        public int depositedCount { get; private set; } = 0;


        public GuildMember(Hashtable info) { ParseInfo(info); }

        public void ParseInfo(Hashtable info) {
            login = info.GetValueString((int)SPC.Login);
            gameRefID = info.GetValueString((int)SPC.GameRefId);
            characterID = info.GetValueString((int)SPC.CharacterId);
            guildStatus = info.GetValueInt((int)SPC.Status);
            exp = info.GetValueInt((int)SPC.Exp);
            characterName = info.GetValueString((int)SPC.CharacterName);
            characterIcon = info.GetValueInt((int)SPC.Icon);
            depositedCount = info.GetValueInt((int)SPC.DepositedCount, 0);
        }

        public bool isOwner {
            get {
                return (guildStatus == (int)GuildMemberStatus.Owner);
            }
        }

        public bool isModerator {
            get {
                return (guildStatus == (int)GuildMemberStatus.Moderator);
            }
        }

        public bool isMember {
            get {
                return (guildStatus == (int)GuildMemberStatus.Member);
            }
        }

        public bool GrantedAddMember() {
            return (guildStatus == (int)GuildMemberStatus.Moderator) || (guildStatus == (int)GuildMemberStatus.Owner);
        }

        public bool GrantedSetDescription() {
            return (guildStatus == (int)GuildMemberStatus.Moderator) || (guildStatus == (int)GuildMemberStatus.Owner);
        }

        public bool hasIcon {
            get {
                return (characterIcon >= 0);
            }
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
