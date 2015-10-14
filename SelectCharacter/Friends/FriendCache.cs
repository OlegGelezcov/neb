using NebulaCommon;
using System.Collections.Concurrent;

namespace SelectCharacter.Friends {
    public class FriendCache {

        private readonly ConcurrentDictionary<string, DbObjectWrapper<PlayerFriends>> mCachedFriends = new ConcurrentDictionary<string, DbObjectWrapper<PlayerFriends>>();

        public bool TryGetPlayerFriends(string gameRefID, out DbObjectWrapper<PlayerFriends> friends) {
            return mCachedFriends.TryGetValue(gameRefID, out friends);
        }

        public bool TryAddPlayerFriends(PlayerFriends friends) {
            if(!ContainsPlayerFriends(friends.gameRefID)) {
                return mCachedFriends.TryAdd(friends.gameRefID, new DbObjectWrapper<PlayerFriends> { Changed = true, Data = friends });
            }
            return false;
        }

        public bool ContainsPlayerFriends(string gameRefID) {
            return mCachedFriends.ContainsKey(gameRefID);
        }

        public void Save(SelectCharacterApplication app) {
            foreach(var pFriends in mCachedFriends) {
                if(pFriends.Value.Changed) {
                    app.DB.friends.Save(pFriends.Value.Data);
                    pFriends.Value.Changed = false;
                }
            }
        }
    }
}
