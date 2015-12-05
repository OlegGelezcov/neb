using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Election {
    public class CommanderCandidate : IInfoParser {

        public Race race { get; private set; }
        public string login { get; private set; }
        public string gameRefID { get; private set; }
        public string characterID { get; private set; }
        public int voices { get; private set; }
        public string guildName { get; private set; }


        public void ParseInfo(Hashtable info) {
            race = (Race)(byte)info.GetValueInt((int)SPC.Race, (int)(byte)Race.Humans);
            login = info.GetValueString((int)SPC.Login);
            gameRefID = info.GetValueString((int)SPC.GameRefId);
            characterID = info.GetValueString((int)SPC.CharacterId);
            voices = info.GetValueInt((int)SPC.Voices);
            guildName = info.GetValueString((int)SPC.Guild);
        }


        public CommanderCandidate(Hashtable info) {
            ParseInfo(info);
        }

    }
}
