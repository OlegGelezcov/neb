using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client.Races {
    public class RaceStat : IInfoParser {
        public Race race { get; private set; }
        public int points { get; private set; }

        public void ParseInfo(Hashtable info) {
            race = (Race)(byte)info.GetValue<int>((int)SPC.Race, (int)(byte)Race.Humans);
            points = info.GetValue<int>((int)SPC.Points, 0);
        }

        public RaceStat(Race race) {
            this.race = race;
            points = 0;
        }

        public RaceStat(Hashtable info) {
            ParseInfo(info);
        }
    }
}
