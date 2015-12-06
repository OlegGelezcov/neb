using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Races {
    public class RaceStat : IInfoParser {
        public Race race { get; private set; }
        public int points { get; private set; }

        public void ParseInfo(Hashtable info) {
            race = (Race)(byte)info.GetValueInt((int)SPC.Race, (int)(byte)Race.Humans);
            points = info.GetValueInt((int)SPC.Points);
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
