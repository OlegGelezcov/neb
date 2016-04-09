using Common;
using ServerClientCommon;
using System.Collections;

namespace SelectCharacter.Guilds {
    public class GuildMember {
        public string login { get; set; }
        public string gameRefId { get; set; }
        public string characterId { get; set; }
        public string characterName { get; set; } = string.Empty;
        public int characterIcon { get; set; } = -1;


        public int guildStatus { get; set; }

        public int exp { get; set; }

        public int lastCacheTime = 0;

        /// <summary>
        /// Is user moderator or owner
        /// </summary>
        public bool IsAddMemberGranted() {
            return (guildStatus == (int)GuildMemberStatus.Moderator) || (guildStatus == (int)GuildMemberStatus.Owner);
        }

        /// <summary>
        /// Is user moderator or owner
        /// </summary>
        public bool IsSetGuildDescriptionGranted() {
            return (guildStatus == (int)GuildMemberStatus.Moderator) || (guildStatus == (int)GuildMemberStatus.Owner);
        }

        public Hashtable GetInfo(SelectCharacterApplication app) {

            int currentTime = CommonUtils.SecondsFrom1970();
            if(currentTime - lastCacheTime > 60 * 60) {
                lastCacheTime = currentTime;
                var player = app.Players.GetExistingPlayer(gameRefId);
                if(player != null ) {
                    var character = player.Data.GetCharacter(characterId);
                    if(character != null ) {
                        exp = character.Exp;
                        characterIcon = character.characterIcon;
                        if(string.IsNullOrEmpty(characterName)) {
                            characterName = character.Name;
                        }
                    }
                }
            }

            //if empty name force get name
            if(string.IsNullOrEmpty(characterName)) {
                var player = app.Players.GetExistingPlayer(gameRefId);
                if(player != null ) {
                    var character = player.Data.GetCharacter(characterId);
                    if(character != null ) {
                        characterName = character.Name;
                    }
                }
            }

            return new Hashtable {
                { (int)SPC.Login, login },
                { (int)SPC.GameRefId, gameRefId },
                { (int)SPC.CharacterId, characterId },
                { (int)SPC.Status, guildStatus },
                { (int)SPC.Exp, exp },
                { (int)SPC.CharacterName, characterName },
                { (int)SPC.Icon, characterIcon }
            };
        }
    }
}
