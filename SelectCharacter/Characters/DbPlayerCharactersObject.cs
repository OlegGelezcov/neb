using Common;
using MongoDB.Bson;
using NebulaCommon.SelectCharacter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;
using ExitGames.Logging;

namespace SelectCharacter.Characters {

    public class DbPlayerCharactersObject : IInfoSource {


        private static readonly ILogger log = LogManager.GetCurrentClassLogger();



        /// <summary>
        /// Unique if of DB object
        /// </summary>
        public ObjectId Id { get; set; }

        /// <summary>
        /// Display name of character
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Player in game id
        /// </summary>
        public string GameRefId { get; set; }

        /// <summary>
        /// Selected character id
        /// </summary>
        public string SelectedCharacterId { get; set; }

        /// <summary>
        /// List of all player characters
        /// </summary>
        public List<DbPlayerCharacter> Characters { get; set; }

        /// <summary>
        /// Update only modules for character
        /// </summary>
        /// <param name="characterId">Id of updated character</param>
        /// <param name="model">New modules dictionary</param>
        public void UpdateModel(string characterId, Dictionary<ShipModelSlotType, string> model, string worldId, int exp) {
            if(this.Characters == null ) {
                return;
            }
            foreach(var character in this.Characters) {
                if(character.CharacterId == characterId) {
                    foreach(var module in model) {
                        character.SetModule((int)module.Key, module.Value);
                    }
                    character.WorldId = worldId;
                    character.Exp = exp;
                    //log.InfoFormat("set world = {0} for character = {1}", character.WorldId, character.CharacterId);
                    log.InfoFormat("update character occured from GameServer: ID={0}, WORLD={1},EXP={2},MODEL={3}",
                        characterId, worldId, exp, model.toHash().ToStringBuilder().ToString());
                    break;
                }
            }
        }



        /// <summary>
        /// Create new character, select it and return new character object
        /// </summary>
        /// <param name="name">Name of character</param>
        /// <param name="level">Level of character</param>
        /// <param name="race">Race of character</param>
        /// <param name="workshop">Home workshop of character</param>
        /// <param name="model">Modules dictionary of character</param>
        /// <param name="exp">Initial exp of character</param>
        /// <returns></returns>
        public DbPlayerCharacter AddCharacter(string name, int race, int workshop, Dictionary<ShipModelSlotType, string> model) {
            if(this.Characters == null) {
                this.Characters = new List<DbPlayerCharacter>();
            }

            DbPlayerCharacter character = new DbPlayerCharacter {
                CharacterId = Guid.NewGuid().ToString(),
                Name = name,
                Deleted = false,
                Race = race,
                Workshop = workshop,
                Exp = 0,
                WorldId = SelectCharacterApplication.Instance.GetStartLocation((Race)race, (Workshop)workshop),
                raceStatus = (int)RaceStatus.None
            };
            character.SetModel(ToHash(model));
            this.Characters.Add(character);
            //select new created character to be selected character
            this.SelectedCharacterId = character.CharacterId;
            //return new character
            return character;
        }

        private Hashtable ToHash(Dictionary<ShipModelSlotType, string> model) {
            Hashtable result = new Hashtable();
            foreach(var kv in model) {
                result.Add((int)kv.Key, kv.Value);
            }
            return result;
        }

        /// <summary>
        /// Find character at set it as selected
        /// </summary>
        /// <param name="characterId">Character id to be selected</param>
        /// <returns>Return new selected character object</returns>
        public DbPlayerCharacter SelectCharacter(string characterId) {
            var character = GetCharacter(characterId);
            if (character != null) {
                this.SelectedCharacterId = character.CharacterId;
            }
            return character;
        }

        /// <summary>
        /// Get existing character from character list
        /// </summary>
        /// <param name="characterId">Character id to find</param>
        /// <returns>Return founded character id or zero</returns>
        public DbPlayerCharacter GetCharacter(string characterId) {
            this.CheckForNonNullCharactersList();
            return this.Characters.Where(ch => ch.CharacterId == characterId).FirstOrDefault();
        }

        /// <summary>
        /// Delete character, and unset selected character
        /// </summary>
        /// <param name="characterId">Character id to be deleted</param>
        public bool DeleteCharacter(string characterId) {

            bool removed = false;

            this.CheckForNonNullCharactersList();
            //first remove character
            var character = this.Characters.Where(c => c.CharacterId == characterId).FirstOrDefault();
            if (character != null) {
                this.Characters.Remove(character);
                //next clear selected character id
                this.SelectedCharacterId = string.Empty;
                removed = true;
            }
            return removed;
        }

        /// <summary>
        /// Force to be character list not null
        /// </summary>
        private void CheckForNonNullCharactersList() {
            if (this.Characters == null) {
                this.Characters = new List<DbPlayerCharacter>();
            }
            if(this.SelectedCharacterId == null) {
                this.SelectedCharacterId = string.Empty;
            }
        }

        /// <summary>
        /// Represent this object in network transfer form
        /// </summary>
        /// <returns></returns>
        public Hashtable GetInfo() {
            this.CheckForNonNullCharactersList();

            var result = new Hashtable {
                {(int)SPC.GameRefId, this.GameRefId },
                {(int)SPC.DisplayName, this.Login },
                {(int)SPC.SelectedCharacterId, this.SelectedCharacterId  }
            };

            var charactersHash = new Hashtable();
            foreach(var character in this.Characters ) {
                charactersHash.Add(character.CharacterId, character.GetInfo());
            }
            result.Add((int)SPC.Characters, charactersHash);
            return result;
        }

        public void SetGuild(string characterID, string guildID) {
            var character = GetCharacter(characterID);
            if(character != null ) {
                character.SetGuild(guildID);
            }
        }

     
        public bool SetRaceStatus(string characterID, int raceStatus) {
            var character = GetCharacter(characterID);
            if(character == null ) {
                return false;
            }
            character.raceStatus = raceStatus;
            return true;
        }
    }
}
