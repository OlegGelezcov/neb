using Common;
using System.Collections;
using System.Collections.Concurrent;

namespace SelectCharacter.Group {
    public class GroupCache {
        public const int GROUP_SEARCH_UPDATE_INTERVAL = 60;

        private ConcurrentDictionary<string, NebulaCommon.Group.Group> groups;
        private int mLastGroupSerachUpdateTime = 0;
        private ConcurrentDictionary<byte, Hashtable> mCacheFoundedGroups;

        public GroupCache() {
            groups = new ConcurrentDictionary<string, NebulaCommon.Group.Group>();
            mCacheFoundedGroups = new ConcurrentDictionary<byte, Hashtable>();
            mCacheFoundedGroups.TryAdd((byte)Race.Humans, new Hashtable());
            mCacheFoundedGroups.TryAdd((byte)Race.Borguzands, new Hashtable());
            mCacheFoundedGroups.TryAdd((byte)Race.Criptizoids, new Hashtable());
        }


        public bool TryGetGroup(string groupID, out NebulaCommon.Group.Group group ) {
            return groups.TryGetValue(groupID, out group);
        }

        public bool TryGetGroupForCharacter(string characterID, out NebulaCommon.Group.Group group ) {
            foreach(var pair in groups) {
                foreach(var mem in pair.Value.members) {
                    if(mem.Key == characterID) {
                        group = pair.Value;
                        return true;
                    }
                }
            }
            group = null;
            return false;
        }

        public bool TryAddGroup(NebulaCommon.Group.Group group ) {
            return groups.TryAdd(group.groupID, group);
        }

        public bool TryRemoveGroup(string groupID ) {
            NebulaCommon.Group.Group removedGroup;
            return groups.TryRemove(groupID, out removedGroup);
        }

        public Hashtable SearchGroups(SelectCharacterApplication app, byte race) {
            var currentTime = CommonUtils.SecondsFrom1970();
            if(currentTime - mLastGroupSerachUpdateTime > GROUP_SEARCH_UPDATE_INTERVAL) {
                mLastGroupSerachUpdateTime = currentTime;
                Hashtable raceGroups = null;
                if (mCacheFoundedGroups.TryGetValue(race, out raceGroups)) {
                    raceGroups.Clear();
                    foreach (var pGroup in groups) {
                        var group = pGroup.Value;
                        if (group.allowNewMembers && group.opened) {

                            if(GetLeaderRace(app, group) == race) {
                                raceGroups.Add(group.groupID, group.GetSearchInfo());
                            }

                        }
                    }
                    return raceGroups;
                } else {
                    return new Hashtable();
                }
            }

            Hashtable result = null;
            if(mCacheFoundedGroups.TryGetValue(race, out result)) {
                return result;
            }
            return new Hashtable();
        }

        private byte GetLeaderRace(SelectCharacterApplication app, NebulaCommon.Group.Group group) {
            var leader = group.leaderMember;
            if(leader != null) {
                var character = app.Players.GetCharacter(leader.gameRefID, leader.characterID);
                if(character != null ) {
                    return (byte)character.Race;
                }
            }
            return (byte)Race.None;
        }
    }
}
