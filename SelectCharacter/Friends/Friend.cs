using Common;
using ServerClientCommon;
using System.Collections;

namespace SelectCharacter.Friends {
    public class Friend  {

        public const int FRIEND_INFO_UPDATE_INTERVAL = 5 * 60 * 60;

        public string login { get; set; }
        public string gameRefID { get; set; }

        public int exp { get; set; } = 0;
        public string characterID { get; set; } = string.Empty;
        public string characterName { get; set; } = string.Empty;
        public string worldID { get; set; } = string.Empty;
        public int lastCacheTime { get; set; } = 0;
        public int characterIcon { get; set; } = -1;

        public Hashtable GetInfo(SelectCharacterApplication app) {
            var currentTime = CommonUtils.SecondsFrom1970();
            if((currentTime - lastCacheTime) > FRIEND_INFO_UPDATE_INTERVAL ) {
                lastCacheTime = currentTime;
                var player = app.Players.GetExistingPlayer(gameRefID);
                if(player != null) {
                    var character = player.Data.GetCharacter(player.Data.SelectedCharacterId);
                    if(character != null ) {
                        exp = character.Exp;
                        characterName = character.Name;
                        worldID = character.WorldId;
                        characterID = character.CharacterId;
                        characterIcon = character.characterIcon;
                        if(characterName == null) {
                            characterName = string.Empty;
                        }
                        if(worldID == null ) {
                            worldID = string.Empty;
                        }
                        
                    }
                }
            }

            return new Hashtable {
                {(int)SPC.Login, login },
                {(int)SPC.GameRefId, gameRefID },
                {(int)SPC.Exp, exp },
                {(int)SPC.CharacterName, characterName },
                {(int)SPC.WorldId, worldID },
                {(int)SPC.CharacterId, characterID },
                {(int)SPC.Icon, characterIcon }
            };
        }
    }
}
