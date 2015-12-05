using Common;
using ExitGames.Client.Photon;
using System.Collections.Generic;

namespace Nebula.Client.Groups {
    public class FoundedGroupCollection : IInfoParser {
        public Dictionary<string, FoundedGroup> groups { get; private set; }

        public FoundedGroupCollection() {
            groups = new Dictionary<string, FoundedGroup>();
        }

        public FoundedGroupCollection(Hashtable info) {
            groups = new Dictionary<string, FoundedGroup>();
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            groups.Clear();
            foreach (System.Collections.DictionaryEntry entry in info) {
                string groupID = (string)entry.Key;
                Hashtable groupInfo = entry.Value as Hashtable;
                if (groupInfo != null) {
                    groups.Add(groupID, new FoundedGroup(groupInfo));
                }
            }
        }
    }
}
