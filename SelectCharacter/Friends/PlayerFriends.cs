using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Friends {
    public class PlayerFriends {
        public ObjectId Id { get; set; }

        public string gameRefID { get; set; } = string.Empty;
        public string login { get; set; } = string.Empty;
        public Dictionary<string, Friend> friends { get; set; } = new Dictionary<string, Friend>();

        private readonly object syncRoot = new object();


        public bool IsFriend(string gameRefID) {
            lock(syncRoot) {
                if(friends.ContainsKey(gameRefID)) {
                    return true;
                }
                return false;
            }
        }

        public Hashtable GetInfo(SelectCharacterApplication app) {
            
            lock(syncRoot) {
                Hashtable result = new Hashtable();
                foreach (var pFriend in friends) {
                    result.Add(pFriend.Key, pFriend.Value.GetInfo(app));
                }
                return result;
            }
            
        }

        public bool AddFriend(string grfID, string login) {
            CheckFriendsNotNull();

            if(!IsFriend(grfID)) {
                lock(syncRoot) {
                    friends.Add(grfID, new Friend { gameRefID = grfID, login = login });
                    return true;
                }
            }
            return false;
        }

        public bool RemoveFriend(string grfID) {
            CheckFriendsNotNull();

            if(IsFriend(grfID)) {
                return friends.Remove(grfID);
            }
            return false;
        }

        private void CheckFriendsNotNull() {
            lock (syncRoot) {
                if (friends == null) {
                    friends = new Dictionary<string, Friend>();
                }
            }
        }

        public bool IsFriend(PlayerFriends other) {
            return IsFriend(other.gameRefID);
        }
    }
}
