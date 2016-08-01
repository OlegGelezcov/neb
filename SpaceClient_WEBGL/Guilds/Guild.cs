using Common;
using ServerClientCommon;
using System;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using Nebula.Client.Utils;

namespace Nebula.Client.Guilds {
    public class Guild : IInfoParser {
        public string ownerCharacterID { get; private set; }
        public int guildRace { get; private set; }
        public int rating { get; private set; }
        public string description { get; private set; }
        public Dictionary<string, GuildMember> members { get; private set; }
        public string name { get; private set; }
        public bool opened { get; private set; }
        public int moderCount { get; private set; }
        public int maxModerCount { get; private set; }
        public int depositedCredits { get; private set; }
        public int depositedPvpPoints { get; private set; }
        public string poster { get; private set; } = string.Empty;
        public List<CoalitionTransaction> transactions { get; private set; } = new List<CoalitionTransaction>();

        public Guild() { }

        public Guild(Hashtable info) { ParseInfo(info); }

        public void Clear() {
            ownerCharacterID = string.Empty;
            rating = 0;
            description = string.Empty;
            if (members != null) {
                members.Clear();
            }
            name = string.Empty;
        }

        public void ParseInfo(Hashtable info) {

            if (members == null) {
                members = new Dictionary<string, GuildMember>();
            }
            members.Clear();

            SPC lastParameter = SPC.Id;
            try {
                ownerCharacterID = info.GetValueString((int)SPC.Id);

                lastParameter = SPC.GuildRating;
                rating = info.GetValueInt((int)SPC.GuildRating);

                lastParameter = SPC.Race;
                guildRace = info.GetValueInt((int)SPC.Race, (int)Race.None);

                lastParameter = SPC.Description;
                description = info.GetValueString((int)SPC.Description);

                lastParameter = SPC.Name;
                name = info.GetValueString((int)SPC.Name);

                lastParameter = SPC.ModerCount;
                moderCount = info.GetValueInt((int)SPC.ModerCount);

                lastParameter = SPC.MaxModerCount;
                maxModerCount = info.GetValueInt((int)SPC.MaxModerCount);

                depositedCredits = info.GetValueInt((int)SPC.Credits);

                depositedPvpPoints = info.GetValueInt((int)SPC.PvpPoints);

            } catch (Exception exception) {
                throw new NebulaException(string.Format("last not handled key = {0}", lastParameter));
            }

            opened = info.GetValueBool((int)SPC.Opened, true);

            Hashtable membersHash = info.GetValueHash((int)SPC.Members) as Hashtable;
            if (membersHash == null) {
                return;
            }

            foreach (System.Collections.DictionaryEntry entry in membersHash) {
                string memberCharacterID = entry.Key.ToString();
                Hashtable memberCharacterInfo = entry.Value as Hashtable;
                members.Add(memberCharacterID, new GuildMember(memberCharacterInfo));
            }

            poster = info.GetValueString((int)SPC.DayPoster);
            
            if(info.ContainsKey((int)SPC.Transactions) ) {
                Hashtable[] arr = info[(int)SPC.Transactions] as Hashtable[];
                if(arr != null ) {
                    transactions = new List<CoalitionTransaction>();
                    foreach(Hashtable h in arr ) {
                        var transaction = CoalitionTransaction.Parse(h);
                        if(transaction != null ) {
                            transactions.Add(transaction);
                        }
                    }
                }
            }
        }

        public bool IsModerator(string characterID) {
            var member = GetMember(characterID);
            if (member == null) {
                return false;
            }
            return (member.guildStatus == (int)GuildMemberStatus.Moderator);
        }

        public GuildMember GetMember(string characterID) {
            if (members == null) {
                return null;
            }

            GuildMember member = null;
            if (members.TryGetValue(characterID, out member)) {
                return member;
            }
            return null;
        }

        public bool HasMember(string characterID) {
            return (GetMember(characterID) != null);
        }

        public bool IsOwner(string characterID) {
            return (characterID == ownerCharacterID);
        }

        public bool IsSoldier(string characterId ) {
            if(HasMember(characterId)) {
                return GetMember(characterId).guildStatus == (int)GuildMemberStatus.Member;
            }
            return false;
        }

        public bool has {
            get {
                return (false == string.IsNullOrEmpty(ownerCharacterID));
            }
        }

        public bool addingModeratorsAllowed {
            get {
                return moderCount < maxModerCount;
            }
        }
    }
}
