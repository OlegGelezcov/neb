using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using Space.Game.UserGroups;
using System.Collections;

namespace Space.Saving {
    public class PlayerFriendsSaveObject {
        public ObjectId Id { get; set; }
        public string GameRefId { get; set; }
        public Hashtable Friends { get; set; }

        public void AddOrReplace(FriendRecord friend) {
            if (this.Friends == null) {
                this.Friends = new Hashtable();
            }
            if (this.Friends.ContainsKey(friend.GameRefId())) {
                this.Friends.Remove(friend.GameRefId());
            }

            this.Friends.Add(friend.GameRefId(), friend.GetInfo());
        }

        public Hashtable Friend(string friendGameRefId) {
            Hashtable result = null;
            if (this.Friends == null) { return null;  }
            foreach (DictionaryEntry entry in this.Friends) {
                if (entry.Key.ToString() == friendGameRefId) {
                    result = entry.Value as Hashtable;
                    break;
                }
            }
            return result;
        }
    }
}
