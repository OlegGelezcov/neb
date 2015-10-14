// FriendBox.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, February 15, 2015 1:34:07 AM
// Copyright (c) 2015 KomarGames. All rights reserved.
//

namespace Space.Game.UserGroups {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Common;
    using System.Collections;

    public class FriendBox : IInfo {
        private Dictionary<string, FriendRecord> friends;

        public FriendBox() {
            this.friends = new Dictionary<string, FriendRecord>();
        }



        public Hashtable GetInfo() {
            Hashtable result = new Hashtable();
            foreach (var pair in this.friends) {
                result.Add(pair.Key, pair.Value.GetInfo());
            }
            return result;
        }

        public void ParseInfo(Hashtable info) {
            friends = new Dictionary<string, FriendRecord>();
            foreach (DictionaryEntry entry in info) {
                Hashtable friendInfo = entry.Value as Hashtable;
                string friendId = friendInfo.GetValueOrDefault<string>(GenericEventProps.game_ref_id);
                if (!string.IsNullOrEmpty(friendId) && !friends.ContainsKey(friendId)) {
                    this.friends.Add(friendId, new FriendRecord(friendInfo));
                }
            }
        }


        public Dictionary<string, FriendRecord> Friends() {
            return this.friends;
        }

        public FriendRecord Friend(string gameRefId) {
            FriendRecord result = null;
            this.friends.TryGetValue(gameRefId, out result);
            return result;
        }
    }
}
