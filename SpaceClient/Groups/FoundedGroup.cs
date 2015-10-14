using Common;
using ServerClientCommon;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Client.Groups {
    public class FoundedGroup : IInfoParser {
        public string groupID { get; private set; }
        public int memberCount { get; private set; }

        public Dictionary<string, FoundedMember> members { get; private set; }


        public void ParseInfo(Hashtable info) {
            members = new Dictionary<string, FoundedMember>();
            groupID = info.GetValue<string>((int)SPC.Group, string.Empty);
            memberCount = info.GetValue<int>((int)SPC.Count, 0);
            Hashtable memberHash = info.GetValue<Hashtable>((int)SPC.Members, new Hashtable());
            if(memberHash != null ) {
                foreach(DictionaryEntry memberEntry in memberHash) {
                    string characterID = (string)memberEntry.Key;
                    Hashtable memberInfo = memberEntry.Value as Hashtable;
                    if(memberInfo != null ) {
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
