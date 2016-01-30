using Common;
using ExitGames.Logging;
using NebulaCommon;
using System.Collections.Generic;

namespace SelectCharacter.Characters {


    public class PlayerCache : Dictionary<string, DbObjectWrapper<DbPlayerCharactersObject>> {

        private readonly object syncObject = new object();
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public bool TryGetPlayerBySelectedCharacterName(string characterName, out DbObjectWrapper<DbPlayerCharactersObject> result ) {
            result = null;
            lock(syncObject ) {
                foreach(var pair in this ) {
                    if(pair.Value.Data.HasSelectedCharacterName(characterName)) {
                        result = pair.Value;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TryGetPlayerByCharacterId(string characterId, out DbObjectWrapper<DbPlayerCharactersObject> result ) {
            result = null;
            lock(syncObject) {
                foreach(var pair in this) {
                    if(pair.Value.Data.HasCharacter(characterId)) {
                        result = pair.Value;
                        return true;
                    }
                }
            }
            return false;
        }


        public bool TryGetPlayerByLogin(string inlogin, out DbObjectWrapper<DbPlayerCharactersObject> result) {
            string login = inlogin.ToLower();
            result = null;
            lock(syncObject) {
                foreach(var pair in this ) {
                    if(pair.Value.Data.Login.ToLower() == login) {
                        result = pair.Value;
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Try get data only tith unsaved changes
        /// </summary>
        public bool TryGetChangedPlayerCharactersObject(string gameRefId, out DbPlayerCharactersObject result ) {
            result = null;

            lock (syncObject) {
                DbObjectWrapper<DbPlayerCharactersObject> wrapper;
                if (TryGetValue(gameRefId, out wrapper)) {
                    if (wrapper.Data != null) {
                        if (wrapper.Changed) {
                            result = wrapper.Data;
                            return true;
                        }
                    } else {
                        Remove(gameRefId);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Try get data from collection
        /// </summary>
        public bool TryGetPlayerCharactersObject(string gameRefId, out DbPlayerCharactersObject result ) {
            result = null;

            lock(syncObject) {
                DbObjectWrapper<DbPlayerCharactersObject> wrapper;
                if(TryGetValue(gameRefId, out wrapper)) {
                    if(wrapper.Data != null ) {
                        result = wrapper.Data;
                        return true;
                    } else {
                        Remove(gameRefId);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// set object for game ref id
        /// </summary>
        public void SetPlayerCharactersObject( string gameRefId, DbPlayerCharactersObject document, bool modified = false) {
            lock(syncObject) {
                if(ContainsKey(gameRefId)) {
                    Remove(gameRefId);
                }

                Add(gameRefId, new DbObjectWrapper<DbPlayerCharactersObject> { Changed = modified, Data = document });
            }
        }

        /// <summary>
        /// Update model, world and exp for character in collection( if present in collection)
        /// </summary>
        /// <param name="gameRefId">id of player</param>
        /// <param name="characterId">id of character</param>
        /// <param name="model">ship model hash</param>
        /// <param name="worldId">current world</param>
        /// <param name="exp">current exp</param>
        public void UpdatePlayerCharacterObject(string gameRefId, string characterId, Dictionary<ShipModelSlotType, string> model, string worldId, int exp) {
            DbObjectWrapper<DbPlayerCharactersObject> wrapper = null;
            lock(syncObject) {
                if (TryGetValue(gameRefId, out wrapper)) {
                    if(wrapper.Data == null ) {
                        log.Error("Wrapper data is null");
                        return;
                    }
                    wrapper.Data.UpdateModel(characterId, model, worldId, exp);
                    wrapper.Changed = true;
                }
            }

        }
        /// <summary>
        /// Delete character from player
        /// </summary>
        /// <param name="gameRefId">id of player</param>
        /// <param name="characterId">id of character</param>
        /// <returns></returns>
        public bool RemovePlayerCharacter(string gameRefId, string characterId ) {
            DbObjectWrapper<DbPlayerCharactersObject> wrapper = null;
            lock(syncObject) {
                if(TryGetValue(gameRefId, out wrapper)) {
                    if(wrapper.Data.DeleteCharacter(characterId)) {
                        wrapper.Changed = true;
                        return true;
                    }
                }
                return false;
            }
        }


        public bool SelectCharacter(string gameRefId, string characterId, out DbPlayerCharactersObject playerObject ) {
            playerObject = null;
            DbObjectWrapper<DbPlayerCharactersObject> wrapper = null;
            lock(syncObject) {
                if(TryGetValue(gameRefId, out wrapper)) {
                    var selectedCharacter = wrapper.Data.SelectCharacter(characterId);
                    if(selectedCharacter == null ) {
                        return false;
                    } else {
                        wrapper.Changed = true;
                        playerObject = wrapper.Data;
                        return true;
                    }
                }
                return false;
            }
         }

        public void UpdateShipModel(string gameRefID, string characterID, ShipModelSlotType slotType, string templateID) {
            DbObjectWrapper<DbPlayerCharactersObject> wrapper = null;
            lock(syncObject) {
                if(TryGetValue(gameRefID, out wrapper)) {
                    if(wrapper.Data.SelectedCharacterId == characterID) {
                        var character = wrapper.Data.GetCharacter(characterID);
                        if(character != null ) {
                            character.SetModule((byte)slotType, templateID);
                            wrapper.Changed = true;
                        }
                    }
                }
            }
        }

        ///// <summary>
        ///// Set race status of character
        ///// </summary>
        ///// <param name="gameRefID">ID of player</param>
        ///// <param name="characterID">Character ID of player character</param>
        ///// <param name="raceStatus">Race status of character</param>
        //public bool SetRaceStatus(string gameRefID, string characterID, int raceStatus) {
        //    DbObjectWrapper<DbPlayerCharactersObject> wrapper = null;
        //    lock(syncObject) {
        //        if(TryGetValue(gameRefID, out wrapper)) {
        //            var character = wrapper.Data.GetCharacter(characterID);
        //            if(character == null ) {
        //                return false;
        //            }
        //            character.raceStatus = raceStatus;
        //            wrapper.Changed = true;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public void RemovePlayerObject(string gameRefId) {
            lock (syncObject) {
                Remove(gameRefId);
            }
        }

        public void SetChanged(string gameRefId, bool changed) {
            DbObjectWrapper<DbPlayerCharactersObject> wrapper = null;
            lock(syncObject) {
                if(TryGetValue(gameRefId, out wrapper)) {
                    wrapper.Changed = changed;
                }
            }
        }

        public void SaveModified(DbReader db) {
            //log.InfoFormat("SaveModified() called");
            lock(syncObject) {
                foreach(var wrapper in this) {
                    if(wrapper.Value.Changed && wrapper.Value.Data != null ) {
                        db.Save(wrapper.Value.Data);
                        wrapper.Value.Changed = false;
                    }
                }
            }
        }
    }

    public class MiniPlayerInfo {
        public int Level;
        public int Exp;
    }
}
