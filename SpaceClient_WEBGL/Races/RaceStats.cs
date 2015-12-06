using Common;
using System.Collections.Generic;
using ExitGames.Client.Photon;


namespace Nebula.Client.Races {
    public class RaceStats : IInfoParser {
        public Dictionary<Race, RaceStat> stats { get; private set; }

        public RaceStats() {
            stats = new Dictionary<Race, RaceStat>() {
                { Race.Humans, new RaceStat(Race.Humans) },
                { Race.Borguzands, new RaceStat(Race.Borguzands )},
                { Race.Criptizoids, new RaceStat(Race.Criptizoids)}
            };
        }

        public RaceStats(Hashtable info) {
            stats = new Dictionary<Race, RaceStat>();
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            stats.Clear();
            foreach (System.Collections.DictionaryEntry raceStatEntry in info) {
                Race race = (Race)(byte)(int)raceStatEntry.Key;
                Hashtable stat = raceStatEntry.Value as Hashtable;
                stats.Add(race, new RaceStat(stat));
            }
        }

        public bool HasStat(Race race) {
            return stats.ContainsKey(race);
        }

        public RaceStat GetStat(Race race) {
            if (HasStat(race)) {
                return stats[race];
            }
            return new RaceStat(race);
        }
    }
}
