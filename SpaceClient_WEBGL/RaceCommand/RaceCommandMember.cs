using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.RaceCommand {
    public class RaceCommandMember : IInfoParser {
        public string login { get; private set; }
        public string characterID { get; private set; }
        public string gameRefID { get; private set; }
        public int exp { get; private set; }
        public Workshop workshop { get; private set; }
        public string characterName { get; private set; }
        public int commandKey { get; private set; }

        public RaceCommandMember(int commandKey, Hashtable info) {
            this.commandKey = commandKey;
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            login = info.GetValueString((int)SPC.Login);
            gameRefID = info.GetValueString((int)SPC.GameRefId);
            characterID = info.GetValueString((int)SPC.CharacterId);
            exp = info.GetValueInt((int)SPC.Exp);
            workshop = (Workshop)(byte)info.GetValueInt((int)SPC.Workshop, (int)(byte)Workshop.Arlen);
            characterName = info.GetValueString((int)SPC.CharacterName);
        }

        public bool has {
            get {
                return !(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(characterID) || string.IsNullOrEmpty(gameRefID));
            }
        }
    }
}
