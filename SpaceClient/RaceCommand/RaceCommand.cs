using Common;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Client.RaceCommand {
    public class RaceCommand : IInfoParser {

        public  Race race { get; private set; }
        public  Dictionary<int, RaceCommandMember> members { get; private set; }

        public RaceCommand(Race inrace, Hashtable memberInfo) {
            race = race;
            members = new Dictionary<int, RaceCommandMember>();
            ParseInfo(memberInfo);
        }

        public void ParseInfo(Hashtable info) {
            members.Clear();
            foreach(DictionaryEntry memberEntry in info) {
                int commandKey = (int)memberEntry.Key;
                Hashtable memberInfo = memberEntry.Value as Hashtable;
                members.Add(commandKey, new RaceCommandMember(commandKey, memberInfo));
            }
        }

        public bool CharacterInCommand(string characterID) {
            if(members == null ) {
                return false;
            }

            foreach(var pMember in members) {
                if(pMember.Value.characterID == characterID ) {
                    return true;
                }
            }

            return false;
        }

        public bool hasCommander {
            get {
                return members.ContainsKey(RaceCommandKey.COMMANDER);
            }
        }

        public bool hasFirstAdmiral {
            get {
                return members.ContainsKey(RaceCommandKey.ADMIRAL1);
            }
        }

        public bool hasSecondAdmiral {
            get {
                return members.ContainsKey(RaceCommandKey.ADMIRAL2);
            }
        }

 

        public RaceCommandMember GetCommander() {
            if(hasCommander) {
                return members[RaceCommandKey.COMMANDER];
            }
            return null;
        }

        public RaceCommandMember GetFirstAdmiral() {
            if(hasFirstAdmiral) {
                return members[RaceCommandKey.ADMIRAL1];
            }
            return null;
        }

        public RaceCommandMember GetSecondAdmiral() {
            if(hasSecondAdmiral) {
                return members[RaceCommandKey.ADMIRAL2];
            }
            return null;
        }
    }
}
