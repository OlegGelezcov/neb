using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using Nebula.Client.Utils;

namespace Nebula.Client.Groups {
    public class FoundedGroup : IInfoParser {
        public string groupID { get; private set; }
        public int memberCount { get; private set; }

        public Dictionary<string, FoundedMember> members { get; private set; }


        public void ParseInfo(Hashtable info) {
            members = new Dictionary<string, FoundedMember>();
            groupID = info.GetValueString((int)SPC.Group);
            memberCount = info.GetValueInt((int)SPC.Count);
            Hashtable memberHash = info.GetValueHash((int)SPC.Members);
            if (memberHash != null) {
                foreach (System.Collections.DictionaryEntry memberEntry in memberHash) {
                    string characterID = (string)memberEntry.Key;
                    Hashtable memberInfo = memberEntry.Value as Hashtable;
                    if (memberInfo != null) {
                        members.Add(characterID, new FoundedMember(characterID, memberInfo));
                    }
                }
            }
        }

        public FoundedGroup(Hashtable info) {
            ParseInfo(info);
        }

    }
}
