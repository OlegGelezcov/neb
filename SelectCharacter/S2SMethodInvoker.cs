using Common;
using ExitGames.Logging;
using ServerClientCommon;
using System.Collections;

namespace SelectCharacter {
    public class S2SMethodInvoker {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private SelectCharacterApplication mApplication;

        public S2SMethodInvoker(SelectCharacterApplication application) {
            mApplication = application;
        }

        public object AddCredits(string login, string gameRefID, string characterID, int credits) {
            log.InfoFormat("{0}:{1}:{2} added credits {3}", login, gameRefID, characterID, credits);
            mApplication.Stores.AddCredits(login, gameRefID, characterID, credits);
            return (int)ReturnCode.Ok;
        }

        public object SetCreditsBonus(string characterId, float bonus ) {
            log.InfoFormat("try set credits bonus {0} for character {1}", bonus, characterId);
            return mApplication.Stores.SetCreditsBonus(characterId, bonus);
        }

        public object AddPvpPoints(string login, string gameRef, string character, string guild, byte race, int pvpPoints) {
            log.InfoFormat("{0}:{1} received pvp points [red]", character, pvpPoints);

            mApplication.Stores.AddPvpPoints(login, gameRef, character, pvpPoints);

            if(!string.IsNullOrEmpty(guild)) {
                mApplication.Guilds.AddRating(guild, pvpPoints);
            }

            if(race != (byte)Race.None ) {
                mApplication.raceStats.AddPoints((Race)race, pvpPoints);
            }

            return (int)ReturnCode.Ok;
        }

        public object RequestGuildInfo(string gameRef, string characterID ) {
            var characterObject = mApplication.Players.GetCharacter(gameRef, characterID);
            if(characterObject != null ) {
                if(!string.IsNullOrEmpty(characterObject.guildID)) {
                    var guild = mApplication.Guilds.GetGuild(characterObject.guildID);
                    if( guild != null ) {
                        return new Hashtable {
                            { (int)SPC.Guild, guild.ownerCharacterId },
                            { (int)SPC.Name, guild.name },
                            { (int)SPC.GameRefId, gameRef }
                        };
                    }
                }
            }

            return new Hashtable {
                { (int)SPC.Guild, string.Empty },
                { (int)SPC.Name, string.Empty },
                { (int)SPC.GameRefId, string.Empty }
            };
        }

        public object RequestRaceStatus(string gameRefID, string characterID) {
            var player = mApplication.Players.GetExistingPlayer(gameRefID);
            if(player != null ) {
                var character = player.Data.GetCharacter(characterID);

                
                if(character != null ) {
                    mApplication.SendRaceStatusChanged(gameRefID, characterID, character.raceStatus);
                    return (int)ReturnCode.Ok;
                }
            }
            return (int)ReturnCode.Fatal;
        }

        public object MiningStationUnderAttackNotification(string characterID, string worldID ) {

            string id = "MS_" + worldID;
            Hashtable data = new Hashtable {
                { (int)SPC.WorldId, worldID }
            };
            var notification = mApplication.Notifications.Create(id, "mining_station_attack",
                data, NotficationRespondAction.Delete, NotificationSourceServiceType.Server,
                 NotificationSubType.MiningStationAttack);
            mApplication.Notifications.SetNotificationToCharacter(characterID, notification);
            return (int)ReturnCode.Ok;
        }
    }
}
