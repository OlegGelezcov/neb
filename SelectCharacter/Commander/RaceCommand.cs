using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Commander {
    public class RaceCommand {

        public ObjectId Id { get; set; }

        public int race { get; set; }
        public RaceCommander commander { get; set; } = new RaceCommander();
        public RaceAdmiral firstAdmiral { get; set; } = new RaceAdmiral();
        public RaceAdmiral secondAdmiral { get; set; } = new RaceAdmiral();

        public void Clear() {
            commander.Clear();
            firstAdmiral.Clear();
            secondAdmiral.Clear();
        }

        public bool SetCommander(string login, string gameRefID, string characterID) {
            if (!commander.has) {
                commander.Set(login, gameRefID, characterID);
                return true;
            }
            return false;
        }

        public bool SetAdmiral(string login, string gameRefID, string characterID ) {
            if(!firstAdmiral.has) {
                firstAdmiral.Set(login, gameRefID, characterID);
                return true;
            } else if(!secondAdmiral.has) {
                secondAdmiral.Set(login, gameRefID, characterID);
                return true;
            }
            return false;
        }

        public bool IsAdmiral(string characterID ) {
            if(firstAdmiral.has && firstAdmiral.characterID == characterID ) {
                return true;
            }
            if(secondAdmiral.has && secondAdmiral.characterID == characterID ) {
                return true;
            }
            return false;
        }
    }
}
