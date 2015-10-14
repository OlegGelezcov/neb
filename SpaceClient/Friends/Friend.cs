using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Friends {
    public class Friend  : IInfoParser{
        public string login { get; private set; }
        public string gameRefID { get; private set; }
        public int exp { get; private set; }
        public string characterName { get; private set; }
        public string characterID { get; private set; }
        public string worldID { get; private set; }

        public Friend(Hashtable info) {
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            login = info.GetValue<string>((int)SPC.Login, string.Empty);
            gameRefID = info.GetValue<string>((int)SPC.GameRefId, string.Empty);
            exp = info.GetValue<int>((int)SPC.Exp, 0);
            characterName = info.GetValue<string>((int)SPC.CharacterName, string.Empty);
            worldID = info.GetValue<string>((int)SPC.WorldId, string.Empty);
            characterID = info.GetValue<string>((int)SPC.CharacterId, string.Empty);
        }

        
    }
}
