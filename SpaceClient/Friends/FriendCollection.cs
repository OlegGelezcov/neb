using Common;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Client.Friends {
    public class FriendCollection : IInfoParser {
        //Collection of friends, key - game ref ID, Value - friend
        public Dictionary<string, Friend> friends { get; private set; }

        public FriendCollection() {
            friends = new Dictionary<string, Friend>();
        }

        public FriendCollection(Hashtable info) {
            friends = new Dictionary<string, Friend>();
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable friendHash) {
            friends.Clear();
            foreach(DictionaryEntry friendEntry in friendHash) {
                string gameRefID = (string)friendEntry.Key;
                Hashtable friendInfo = (Hashtable)friendEntry.Value;
                friends.Add(gameRefID, new Friend(friendInfo));
            }
        }

        public int count {
            get {
                return friends.Count;
            }
        }

        public Friend GetFriend(string gameRefID) {
            if(friends.ContainsKey(gameRefID)) {
                return friends[gameRefID];
            }
            return null;
        }

        public bool HasFriend(string gameRefID) {
            return friends.ContainsKey(gameRefID);
        }
    }
}
