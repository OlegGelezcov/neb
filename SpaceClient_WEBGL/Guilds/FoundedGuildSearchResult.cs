using Common;
using ExitGames.Client.Photon;
using System.Collections.Generic;

namespace Nebula.Client.Guilds {
    public class FoundedGuildSearchResult : IInfoParser {
        private readonly Dictionary<string, FoundedGuild> mGuilds;

        public FoundedGuildSearchResult() {
            mGuilds = new Dictionary<string, FoundedGuild>();
        }

        public void ParseInfo(Hashtable info) {
            mGuilds.Clear();
            foreach (System.Collections.DictionaryEntry guildEntry in info) {
                Hashtable guildInfo = guildEntry.Value as Hashtable;
                if (guildInfo != null) {
                    mGuilds.Add(guildEntry.Key.ToString(), new FoundedGuild(guildInfo));
                }
            }
        }

        public Dictionary<string, FoundedGuild> guilds {
            get {
                return mGuilds;
            }
        }
    }
}
