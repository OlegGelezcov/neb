using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client.Election {
    public class CommanderCandidate : IInfoParser {

        public Race race { get; private set; }
        public string login { get; private set; }
        public string gameRefID { get; private set; }
        public string characterID { get; private set; }
        public int voices { get; private set; }
        public string guildName { get; private set; }


        public void ParseInfo(Hashtable info) {
            race = (Race)(byte)info.GetValue<int>((int)SPC.Race, (int)(byte)Race.Humans);
            login = info.GetValue<string>((int)SPC.Login, string.Empty);
            gameRefID = info.GetValue<string>((int)SPC.GameRefId, string.Empty);
            characterID = info.GetValue<string>((int)SPC.CharacterId, string.Empty);
            voices = info.GetValue<int>((int)SPC.Voices, 0);
            guildName = info.GetValue<string>((int)SPC.Guild, string.Empty);
        }


        public CommanderCandidate(Hashtable info) {
            ParseInfo(info);
        }

    }
}
