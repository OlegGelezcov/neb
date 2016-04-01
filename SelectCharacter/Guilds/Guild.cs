using Common;
using GameMath;
using MongoDB.Bson;
using ServerClientCommon;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SelectCharacter.Guilds {
    public class Guild {

        private const int MAX_MEMBERS_ON_PAGE = 300;
        private const int MAX_NUMBER_OF_MODERATORS = 3;

        public ObjectId Id { get; set; }

        public string ownerCharacterId { get; set; }
        public int guildRace { get; set; }
        public Dictionary<string, GuildMember> members { get; set; }
        public int rating { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public bool opened { get; set; } = true;
        public int depositedCredits { get; set; } = 0;
        public int depositedPvpPoints { get; set; } = 0;

        private int mPage = 0;

        public bool closed {
            get {
                return (!opened);
            }
        }

        public int numPages {
            get {
                int memberCount = members.Count;
                if(memberCount % MAX_MEMBERS_ON_PAGE == 0) {
                    return memberCount / MAX_MEMBERS_ON_PAGE;
                } else {
                    return memberCount / MAX_MEMBERS_ON_PAGE + 1;
                }
            }
        }

        public int moderatorCount {
            get {
                lock(syncRoot) {
                    return members.Count(km => km.Value.guildStatus == (int)GuildMemberStatus.Moderator);
                }
            }
        }

        public bool addingModersAllowed {
            get {
                return moderatorCount < MAX_NUMBER_OF_MODERATORS;
            }
        }

        private readonly object syncRoot = new object();

        public bool TryGetMember(string characterId, out GuildMember member) {
            lock(syncRoot) {
                return members.TryGetValue(characterId, out member);
            }
        }

        public ConcurrentBag<GuildMember> GetPrivilegedUsers() {
            ConcurrentBag<GuildMember> privilegedMembers = new ConcurrentBag<GuildMember>();
            lock(syncRoot) {
                foreach(var pMember in members) {
                    if(pMember.Value.IsAddMemberGranted()) {
                        privilegedMembers.Add(pMember.Value);
                    }
                }
            }
            return privilegedMembers;
        }


        public bool AddMember(GuildMember member) {
            lock(syncRoot) {
                if(members.ContainsKey(member.characterId)) {
                    return false;
                }
                members[member.characterId] = member;
                return true;
            }
        }

        public List<string> guildCharacters {
            get {
                List<string> ids = new List<string>();
                if(members != null ) {
                    lock(syncRoot) {
                        foreach(var pair in members) {
                            ids.Add(pair.Key);
                        }
                    }
                }
                return ids;
            }
        }

        public bool RemoveMember(string characterID) {
            lock (syncRoot) {
                return members.Remove(characterID);
            }
        }

        public void AddRating(int points) {
            lock(syncRoot) {
                rating += points;
            }
        }

        public List<GuildMember> memberList {
            get {
                List<GuildMember> result = new List<GuildMember>();
                lock(syncRoot) {

                    foreach(var p in members) {
                        result.Add(p.Value);
                    }
                    return result;
                }
            }
        }

        public void IncrementPage() {
            mPage++;
            mPage = Mathf.Clamp(mPage, 0, numPages - 1);
        }

        public void DecrementPage() {
            mPage--;
            mPage = Mathf.Clamp(mPage, 0, numPages - 1);
        }

        public Hashtable GetInfo(SelectCharacterApplication app) {
            if(description == null) {
                description = string.Empty;
            }

            Hashtable hash = new Hashtable {
                { (int)SPC.Id, ownerCharacterId },
                { (int)SPC.GuildRating, rating },
                { (int)SPC.Race, guildRace },
                { (int)SPC.Description, ( description != null ) ? description : string.Empty },
                { (int)SPC.Name, ( name != null ) ? name : string.Empty  },
                { (int)SPC.Opened, opened },
                { (int)SPC.ModerCount, moderatorCount },
                { (int)SPC.MaxModerCount, MAX_NUMBER_OF_MODERATORS },
                { (int)SPC.Credits, depositedCredits },
                { (int)SPC.PvpPoints, depositedPvpPoints }
            };

            Hashtable membersHash = new Hashtable();
            lock(syncRoot) {
                //mPage = Mathf.Clamp(mPage, 0, numPages - 1);
                //var newMembers = members.Skip(mPage * MAX_MEMBERS_ON_PAGE).Take(MAX_MEMBERS_ON_PAGE).ToDictionary(pk => pk.Key, pk => pk.Value);

                foreach (var member in members ) {
                    membersHash.Add(member.Key, member.Value.GetInfo(app));
                }
            }
            hash.Add((int)SPC.Members, membersHash);
            return hash;
        }

        public void SetOpened(bool inopened) {
            opened = inopened;
        }

        public Hashtable GetShortInfo() {
            int count = 0;
            if(members != null ) { count = members.Count; }

            Hashtable hash = new Hashtable {
                { (int)SPC.Id, ownerCharacterId },
                { (int)SPC.GuildRating, rating },
                { (int)SPC.Race, guildRace },
                { (int)SPC.Description, description },
                { (int)SPC.Name, name },
                { (int)SPC.Count, count },
                { (int)SPC.Opened, opened }
            };
            return hash;
        }


        public bool IsOwner(string characterID) { return characterID == ownerCharacterId; }



        /// <summary>
        /// Add credits to guild
        /// </summary>
        /// <param name="cnt">Credits to add</param>
        public void AddCredits(int cnt) {
            if(cnt > 0 ) {
                depositedCredits += cnt;
            }
        }

        /// <summary>
        /// Add pvp points to guild
        /// </summary>
        /// <param name="cnt">Pvp points to add</param>
        public void AddPvpPoints(int cnt) {
            if(cnt > 0 ) {
                depositedPvpPoints += cnt;
            }
        }

        /// <summary>
        /// Remove credits from guild. Removed value must be less than guild has
        /// </summary>
        /// <param name="cnt">Count credits to remove</param>
        /// <returns>True if remove success, else false</returns>
        public bool RemoveCredits(int cnt) {
            if(depositedCredits >= cnt ) {
                depositedCredits -= cnt;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove pvp points from guild deposit. Removed value must be less or equal than guild has
        /// </summary>
        /// <param name="cnt">Count of pvp points to remove</param>
        /// <returns>True if removed successfully else false</returns>
        public bool RemovePvpPoints(int cnt) {
            if(depositedPvpPoints >= cnt) {
                depositedPvpPoints -= cnt;
                return true;
            }
            return false;
        }
    }
}
