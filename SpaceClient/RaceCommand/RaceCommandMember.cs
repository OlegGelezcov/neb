using Common;
using ServerClientCommon;
using System.Collections;

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
            login           = info.GetValue<string>((int)SPC.Login, string.Empty);
            gameRefID       = info.GetValue<string>((int)SPC.GameRefId, string.Empty);
            characterID     = info.GetValue<string>((int)SPC.CharacterId, string.Empty);
            exp             = info.GetValue<int>((int)SPC.Exp, 0);
            workshop        = (Workshop)(byte)info.GetValue<int>((int)SPC.Workshop, (int)(byte)Workshop.Arlen);
            characterName   = info.GetValue<string>((int)SPC.CharacterName, string.Empty);
        }

        public bool has {
            get {
                return !(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(characterID) || string.IsNullOrEmpty(gameRefID));
            }
        }
    }
}
