using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace NebulaCommon.Group {
    public class Group : IInfo{

        public const int MAX_GROUP_MEMBER_COUNT = 4;

        public string groupID { get; set; }
        public bool opened { get; set; } = true;
        public ConcurrentDictionary<string, GroupMember> members { get; set; } = new ConcurrentDictionary<string, GroupMember>();


        public int memberCount {
            get {
                return members.Count;
            }
        }

        public void Clear() {
            groupID = string.Empty;
            opened = false;
            members.Clear();
        }

        public bool AddMember(GroupMember member) {
            if(members.ContainsKey(member.characterID)) {
                return false;
            }
            return members.TryAdd(member.characterID, member);
        }

        public GroupMember leaderMember {
            get {
                foreach(var pMember in members) {
                    if(pMember.Value.isLeader) {
                        return pMember.Value;
                    }
                }
                return null;
            }
        }

        public bool RemoveMember(string characterID) {
            GroupMember value;
            return members.TryRemove(characterID, out value);
        }

        public Hashtable GetInfo() {
            Hashtable info = new Hashtable { { (int)SPC.Group, groupID }, { (int)SPC.Opened, opened } };


            Hashtable memberHash = new Hashtable();
            foreach(var pair in members) {
                memberHash.Add(pair.Key, pair.Value.GetInfo());
            }

            info.Add((int)SPC.Members, memberHash);
            return info;
        }

        public Hashtable GetSearchInfo() {
            Hashtable membersHash = new Hashtable();
            foreach(var member in members) {
                membersHash.Add(member.Key, new Hashtable {
                    { (int)SPC.Workshop, member.Value.workshop },
                    { (int)SPC.Exp, member.Value.exp }
                });
            }
            return new Hashtable {
                { (int)SPC.Group, groupID },
                { (int)SPC.Count, memberCount},
                { (int)SPC.Members, membersHash}
            };
        }



        public void ParseInfo(Hashtable info) {
            groupID = info.Value<string>((int)SPC.Group);
            opened = info.Value<bool>((int)SPC.Opened);
            Hashtable memberHash = info.Value<Hashtable>((int)SPC.Members);
            members = new ConcurrentDictionary<string, GroupMember>();

            foreach(DictionaryEntry pair in memberHash) {
                GroupMember m = new GroupMember();
                m.ParseInfo(pair.Value as Hashtable);
                members.TryAdd(pair.Key.ToString(), m);
            }
        }

        public bool allowNewMembers {
            get {
                return memberCount < MAX_GROUP_MEMBER_COUNT;
            }
        }

        public bool IsLeader(string characterID) {
            foreach(var member in members) {
                if(member.Value.characterID == characterID) {
                    return member.Value.isLeader;
                }
            }
            return false;
        }

        public bool HasMember(string characterID) {
            return members.ContainsKey(characterID);
        }

        public void SetLeader(string characterID ) {
            if(HasMember(characterID)) {
                foreach(var member in members) {
                    if(member.Value.characterID == characterID) {
                        member.Value.isLeader = true;
                    } else {
                        member.Value.isLeader = false;
                    }
                }
            }
        }

        public void SetRandomLeaderExclude(string characterID) {
            foreach(var member in members) {
                member.Value.isLeader = false;
            }

            foreach(var member in members) {
                if(member.Value.characterID != characterID) {
                    member.Value.isLeader = true;
                    break;
                }
            }
        }

        public GroupMember leader {
            get {
                foreach(var m in members ) {
                    if(m.Value.isLeader) {
                        return m.Value;
                    }
                }
                return null;
            }
        }
    }
}
