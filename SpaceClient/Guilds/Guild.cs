using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Client.Guilds {
    public class Guild  : IInfoParser {
        public string ownerCharacterID { get; private set; }
        public int guildRace { get; private set; }
        public int rating { get; private set; }
        public string description { get; private set; }
        public Dictionary<string, GuildMember> members { get; private set; }
        public string name { get; private set; }
        public bool opened { get; private set; }
        public int moderCount { get; private set; }
        public int maxModerCount { get; private set; }

        public Guild() { }

        public Guild(Hashtable info) { ParseInfo(info); }

        public void Clear() {
            ownerCharacterID = string.Empty;
            rating = 0;
            description = string.Empty;
            if(members != null ) {
                members.Clear();
            }
            name = string.Empty;
        }

        public void ParseInfo(Hashtable info) {

            if(members == null) {
                members = new Dictionary<string, GuildMember>();
            }
            members.Clear();

            SPC lastParameter = SPC.Id;
            try {
                ownerCharacterID = info.GetValue<string>((int)SPC.Id, string.Empty);

                lastParameter = SPC.GuildRating;
                rating = info.GetValue<int>((int)SPC.GuildRating, 0);

                lastParameter = SPC.Race;
                guildRace = info.GetValue<int>((int)SPC.Race, (int)Race.None);

                lastParameter = SPC.Description;
                description = info.GetValue<string>((int)SPC.Description, string.Empty);

                lastParameter = SPC.Name;
                name = info.GetValue<string>((int)SPC.Name, string.Empty);

                lastParameter = SPC.ModerCount;
                moderCount = info.GetValue<int>((int)SPC.ModerCount, 0);

                lastParameter = SPC.MaxModerCount;
                maxModerCount = info.GetValue<int>((int)SPC.MaxModerCount, 0);
            } catch(Exception exception) {
                throw new NebulaException(string.Format("last not handled key = {0}", lastParameter));
            }

            opened = info.GetValue<bool>((int)SPC.Opened, true);

            Hashtable membersHash = info.Value<Hashtable>((int)SPC.Members) as Hashtable;
            if(membersHash == null ) {
                return;
            }

            foreach(DictionaryEntry entry in membersHash ) {
                string memberCharacterID = entry.Key.ToString();
                Hashtable memberCharacterInfo = entry.Value as Hashtable;
                members.Add(memberCharacterID, new GuildMember(memberCharacterInfo));
            }
        }

        public bool IsModerator(string characterID) {
            var member = GetMember(characterID);
            if(member == null ) {
                return false;
            }
            return (member.guildStatus == (int)GuildMemberStatus.Moderator);
        }

        public GuildMember GetMember(string characterID ) {
            if(members == null ) {
                return null;
            }

            GuildMember member = null;
            if(members.TryGetValue(characterID, out member)) {
                return member;
            }
            return null;
        }

        public bool IsMember(string characterID ) {
            return (GetMember(characterID) != null);
        }

        public bool IsOwner(string characterID ) {
            return (characterID == ownerCharacterID);
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
