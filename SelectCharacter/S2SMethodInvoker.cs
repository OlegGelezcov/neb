﻿using Common;
using ExitGames.Logging;
using SelectCharacter.Chat;
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

        public object RequestIcon(string gameRef, string characterId ) {
            var character = mApplication.Players.GetCharacter(gameRef, characterId);
            if(character != null ) {
                return new Hashtable {
                    {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                    {(int)SPC.GameRefId, gameRef  },
                    {(int)SPC.Icon, character.characterIcon  }
                };
            } else {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.CharacterNotFound }
                };
            }
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

        public object MiningStationUnderAttackNotification(string characterID, string worldID, byte race) {

            string id = "MS_" + worldID;
            Hashtable data = new Hashtable {
                { (int)SPC.WorldId, worldID },
                { (int)SPC.Race, (int)race }
            };
            var notification = mApplication.Notifications.Create(id, "mining_station_attack",
                data, NotficationRespondAction.Delete, NotificationSourceServiceType.Server,
                 NotificationSubType.MiningStationAttack);
            mApplication.Notifications.SetNotificationToCharacter(characterID, notification);
            return (int)ReturnCode.Ok;
        }

        public object PlanetObjectUnderAttackNotification(string characterID, string worldID, byte race, int planetObjectType ) {
            string id = "PMS_" + worldID + planetObjectType.ToString() + race.ToString();
            Hashtable data = new Hashtable {
                { (int)SPC.WorldId, worldID },
                { (int)SPC.Race, (int)race },
                { (int)SPC.ObjectType, planetObjectType }
            };
            var notification = mApplication.Notifications.Create(id, "planet_object_attack", data, NotficationRespondAction.Delete, NotificationSourceServiceType.Server,
                 NotificationSubType.PlanetObjectAttack);
            mApplication.Notifications.SetNotificationToCharacter(characterID, notification);
            return (int)ReturnCode.Ok;
        }

        public object SendChatBroadcast(string message) {
            ChatMessage msg = new ChatMessage {
                chatGroup = (int)ChatGroup.race,
                links = new System.Collections.Generic.List<ChatLinkedObject>(),
                message = message,
                messageID = System.Guid.NewGuid().ToString(),
                senderIconId = 1,
                sourceCharacterID = string.Empty,
                sourceCharacterName = "system",
                sourceLogin = string.Empty,
                targetCharacterID = string.Empty,
                targetCharacterName = string.Empty,
                targetLogin = string.Empty,
                time = CommonUtils.SecondsFrom1970()
            };
            mApplication.Chat.SendBroadcast(msg);
            return (int)ReturnCode.Ok;
        }
    }
}
