using Common;
using ServerClientCommon;
using System.Collections;

namespace SelectCharacter.Commander {
    public class RaceCommander : ICommandMember{

        public string login { get; set; } = string.Empty;
        public string gameRefID { get; set; } = string.Empty;
        public string characterID { get; set; } = string.Empty;


        public bool has {
            get {
                return (!string.IsNullOrEmpty(login)) &&
                    (!string.IsNullOrEmpty(gameRefID)) &&
                    (!string.IsNullOrEmpty(characterID));
            }
        }

        public void Clear() {
            login = string.Empty;
            gameRefID = string.Empty;
            characterID = string.Empty;
        }

        public void Set(string login, string gameRefID, string characterID ) {
            this.login = login;
            this.gameRefID = gameRefID;
            this.characterID = characterID;
        }

        public bool IsCommander(string login, string gameRefID, string characterID) {
            return (this.login == login) && (this.gameRefID == gameRefID) && (this.characterID == characterID);
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Login, login },
                { (int)SPC.GameRefId, gameRefID },
                { (int)SPC.CharacterId, characterID }
            };
        }

        public bool exists {
            get {
                return !(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(gameRefID) || string.IsNullOrEmpty(characterID));
            }
        }
    }
}
