using Common;
using ExitGames.Logging;
using MongoDB.Driver.Builders;
using NebulaCommon;
using NebulaCommon.SelectCharacter;
using NebulaCommon.ServerToServer.Events;
using Photon.SocketServer;
using SelectCharacter.Events;
using SelectCharacter.Operations;
using System.Collections;
using System.Collections.Generic;

namespace SelectCharacter.Characters {
    public class PlayerService {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private PlayerCache mCache = new PlayerCache();
        private SelectCharacterApplication mApplication;

        public PlayerService(SelectCharacterApplication application) {
            mApplication = application;
        }

        public bool TryGetPlayer(string gameRefId, out DbPlayerCharactersObject player) {
            return mCache.TryGetPlayerCharactersObject(gameRefId, out player);
        }

        public DbPlayerCharacter GetCharacter(string gameRefID, string characterID) {
            var player = GetExistingPlayer(gameRefID);
            if(player != null ) {
                return player.Data.GetCharacter(characterID);
            }
            return null;
        }

        /// <summary>
        /// Return existing player from cache or database without creating new player
        /// </summary>
        public DbObjectWrapper<DbPlayerCharactersObject> GetExistingPlayer(string gameRefID ) {
            DbObjectWrapper<DbPlayerCharactersObject> player = null;
            if(mCache.TryGetValue(gameRefID, out player)) {
                return player;
            }
            var playerFromDB = mApplication.DB.Get(gameRefID);
            if(playerFromDB != null ) {
                mCache.SetPlayerCharactersObject(gameRefID, playerFromDB);
                if(mCache.TryGetValue(gameRefID, out player)) {
                    return player;
                }
            }
            player = null;
            return player;
        }

        /// <summary>
        /// Set race status of character ID
        /// </summary>
        /// <param name="gameRefID">Game ref ID of character player</param>
        /// <param name="characterID">Character ID of player</param>
        /// <param name="raceStatus">Race status</param>
        /// <returns></returns>
        public bool SetRaceStatus(string gameRefID, string characterID, int raceStatus) {
            var player = GetExistingPlayer(gameRefID);
            if(player == null ) {
                return false;
            }

            if(player.Data.SetRaceStatus(characterID, raceStatus)) {
                player.Changed = true;
                mCache.SetChanged(gameRefID, true);

                var character = player.Data.GetCharacter(characterID);

                bool result = mApplication.RaceCommands.SetRaceStatus(character.Race, (RaceStatus)raceStatus, player.Data.Login, gameRefID, characterID);
                log.InfoFormat("set race status {0} to race = {1}, login = {2}, gameRefID = {3}, character ID = {4}, result = {5}",
                    (RaceStatus)raceStatus, (Race)(byte)character.Race, player.Data.Login, gameRefID, characterID, result);

                mApplication.SendRaceStatusChanged(gameRefID, characterID, raceStatus);
                SendCharacterUpdateToClient(player);

                return true;
            }
            return false;
        }

        private void SendCharacterUpdateToClient(DbObjectWrapper<DbPlayerCharactersObject> player) {
            GenericEvent eventInstance = new GenericEvent {
                subCode = (int)SelectCharacterGenericEventSubCode.CharactersUpdate,
                data = player.Data.GetInfo()
            };
            EventData eventData = new EventData((byte)SelectCharacterEventCode.GenericEvent, eventInstance);

            mApplication.SendEventToClient(player.Data.GameRefId, eventData);
        }

        public DbObjectWrapper<DbPlayerCharactersObject> GetExistingPlayerByLogin(string login) {
            DbObjectWrapper<DbPlayerCharactersObject> player = null;
            if(mCache.TryGetPlayerByLogin(login, out player)) {
                return player;
            }
            var playerFromDB = mApplication.DB.GetByLogin(login);
            if(playerFromDB != null ) {
                mCache.SetPlayerCharactersObject(playerFromDB.GameRefId, playerFromDB);
                if (mCache.TryGetValue(playerFromDB.GameRefId, out player)) {
                    return player;
                }
            }
            player = null;
            return player;
        }





        public DbPlayerCharactersObject GetPlayerCharactersObject(string gameRefId, string login) {

            DbPlayerCharactersObject cachedObject;
            if (mCache.TryGetChangedPlayerCharactersObject(gameRefId, out cachedObject)) {
                mApplication.DB.Save(cachedObject);
            }

            //red object from database
            var objectFromDatabase = mApplication.DB.Get(gameRefId);

            //if not exists, create new and save at database
            if (objectFromDatabase == null) {
                objectFromDatabase = new DbPlayerCharactersObject() {
                    Characters = new List<DbPlayerCharacter>(),
                    Login = login,
                    GameRefId = gameRefId,
                    SelectedCharacterId = string.Empty
                };
                mApplication.DB.Save(objectFromDatabase);

            }
            //update in work collection
            mCache.SetPlayerCharactersObject(gameRefId, objectFromDatabase, false);

            //return object
            return objectFromDatabase;
        }



        public bool DeleteCharacter(string gameRefId, string characterId, out DbPlayerCharactersObject playerCharactersObject) {
            playerCharactersObject = null;
            if (mCache.RemovePlayerCharacter(gameRefId, characterId)) {
                if (mCache.TryGetPlayerCharactersObject(gameRefId, out playerCharactersObject)) {
                    DeleteCharacterInfo(characterId);
                    return true;
                }
            }
            return false;
        }

        private void DeleteCharacterInfo(string characterID) {
            mApplication.DB.characters.Remove(Query<CharacterInfo>.EQ(ch => ch.characterID, characterID));
        }
        private void CreateCharacterInfo(string characterID, string characterName) {
            mApplication.DB.characters.Save(new CharacterInfo { characterID = characterID, characterName = characterName });
        }

        private bool AlreadyExists(string characterName) {
            return (mApplication.DB.characters.FindOne(Query<CharacterInfo>.EQ(ch => ch.characterName, characterName)) != null);
        }

        public OperationResponse CreateCharacter(
                OperationRequest operationRequest, 
                SendParameters sendParameters,
                string gameRefId, 
                string name, 
                Race race, 
                Workshop workshop, 
                int icon,
                out DbPlayerCharactersObject playerObject) {

            playerObject = null;
            if (false == mCache.TryGetPlayerCharactersObject(gameRefId, out playerObject)) {
                return new OperationResponse(operationRequest.OperationCode) {
                    ReturnCode = (short)ReturnCode.Fatal,
                    DebugMessage = "Error of retrieving cached player characters object"
                };
            }

            if (playerObject.Characters.Count >= SelectCharacterSettings.Default.MaxPlayerCharactersCount) {
                return new OperationResponse(operationRequest.OperationCode) {
                    ReturnCode = (short)ReturnCode.Fatal,
                    DebugMessage = "Player already has maximum count of characters"
                };
            }

            if(AlreadyExists(name)) {
                return new OperationResponse(operationRequest.OperationCode) {
                    ReturnCode = (short)ReturnCode.NameAlreadyExists,
                    DebugMessage = name
                };
            }

            var startModules = mApplication.StartModules.StartModuleFor(race, workshop);

            if (startModules == null || startModules.Count != 5) {
                return new OperationResponse(operationRequest.OperationCode) {
                    ReturnCode = (short)ReturnCode.Fatal,
                    DebugMessage = "Error of getting start modules for character"
                };
            }

            var newCharacter = playerObject.AddCharacter(name, (int)race, (int)workshop, startModules, icon );

            CreateCharacterInfo(newCharacter.CharacterId, newCharacter.Name);

            mCache.SetChanged(gameRefId, true);

            CreateCharacterOperationResponse responseObject = new CreateCharacterOperationResponse {
                CharacterId = newCharacter.CharacterId,
                Characters = playerObject.GetInfo()
            };
            return new OperationResponse(operationRequest.OperationCode, responseObject);
        }

        public bool SelectCharacter(string gameRefId, string characterId, out DbPlayerCharactersObject playerObject) {
            return mCache.SelectCharacter(gameRefId, characterId, out playerObject);
        }

        public void UpdateShipModule(string gameRefID, string characterID, ShipModelSlotType slotType, string templateID) {
            mCache.UpdateShipModel(gameRefID, characterID, slotType, templateID);
        }

        public void UpdateCharacter(UpdateCharacterEvent evt) {
            Dictionary<ShipModelSlotType, string> model = new Dictionary<ShipModelSlotType, string>();
            foreach (DictionaryEntry entry in evt.Model) {
                model.Add((ShipModelSlotType)(byte)int.Parse(entry.Key.ToString()), entry.Value.ToString());
            }

            mCache.UpdatePlayerCharacterObject(evt.GameRefId, evt.CharacterId, model, evt.WorldId, evt.Exp);
        }

        /// <summary>
        /// When client peer disconnect call this function for cleanup player collection
        /// </summary>
        /// <param name="gameRefID"></param>
        public void SaveAndRemovePlayerFromCollection(string gameRefID) {
            if (string.IsNullOrEmpty(gameRefID)) {
                log.Info("invalid game ref id");
                return;
            }
            log.InfoFormat("Application.SaveAndRemovePlayer from collection = {0}", gameRefID);
            DbPlayerCharactersObject player;
            if (mCache.TryGetPlayerCharactersObject(gameRefID, out player)) {
                mCache.SetChanged(gameRefID, true);
                mCache.SaveModified(mApplication.DB);
                mCache.RemovePlayerObject(gameRefID);
            }
        }

        public void SaveModified(DbReader db) {
            mCache.SaveModified(db);
        }

        /// <summary>
        /// Set guild for player character
        /// </summary>
        public void SetGuild(string gameRefID, string characterID, string guildID ) {
            DbObjectWrapper<DbPlayerCharactersObject> player = GetExistingPlayer(gameRefID);
            if(player == null ) {
                log.InfoFormat("plaer = {0} not exists", gameRefID);
                return;
            }

            player.Data.SetGuild(characterID, guildID);
            player.Changed = true;

            SelectCharacterClientPeer peer = null;
            if(mApplication.Clients.TryGetPeerForGameRefId(player.Data.GameRefId, out peer)) {
                if(peer != null ) {
                    CharacterUpdateEvent evt = new CharacterUpdateEvent { Characters = player.Data.GetInfo() };
                    EventData eventData = new EventData((byte)SelectCharacterEventCode.CharactersUpdate, evt);
                    peer.SendEvent(eventData, new SendParameters());
                }
            }
        }
    }
}
