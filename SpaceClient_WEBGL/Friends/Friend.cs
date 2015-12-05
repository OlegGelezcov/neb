using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Friends {
    public class Friend : IInfoParser {
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
            login = info.GetValueString((int)SPC.Login);
            gameRefID = info.GetValueString((int)SPC.GameRefId);
            exp = info.GetValueInt((int)SPC.Exp);
            characterName = info.GetValueString((int)SPC.CharacterName);
            worldID = info.GetValueString((int)SPC.WorldId);
            characterID = info.GetValueString((int)SPC.CharacterId);
        }


    }
}
