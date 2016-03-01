using Common;
using ExitGames.Client.Photon;
using System.Collections.Generic;

namespace Nebula.Client.RaceCommand {
    public class RaceCommand : IInfoParser {

        public Race race { get; private set; }
        public Dictionary<int, RaceCommandMember> members { get; private set; }

        public RaceCommand(Race inrace, Hashtable memberInfo) {
            race = race;
            members = new Dictionary<int, RaceCommandMember>();
            ParseInfo(memberInfo);
        }

        public void ParseInfo(Hashtable info) {
            members.Clear();
            foreach (System.Collections.DictionaryEntry memberEntry in info) {
                int commandKey = (int)memberEntry.Key;
                Hashtable memberInfo = memberEntry.Value as Hashtable;
                members.Add(commandKey, new RaceCommandMember(commandKey, memberInfo));
            }
        }

        public bool CharacterInCommand(string characterID) {
            if (members == null) {
                return false;
            }

            foreach (var pMember in members) {
                if (pMember.Value.characterID == characterID) {
                    return true;
                }
            }

            return false;
        }

        public bool hasCommander {
            get {
                if( members.ContainsKey(RaceCommandKey.COMMANDER) ) {
                    var cmd = members[RaceCommandKey.COMMANDER];
                    if(cmd != null ) {
                        return cmd.has;
                    }
                }
                return false;
            }
        }

        public bool hasFirstAdmiral {
            get {
                if( members.ContainsKey(RaceCommandKey.ADMIRAL1) ) {
                    var firstAdmiral = members[RaceCommandKey.ADMIRAL1];
                    if(firstAdmiral != null ) {
                        return firstAdmiral.has;
                    }
                }
                return false;
            }
        }

        public bool hasSecondAdmiral {
            get {
                if( members.ContainsKey(RaceCommandKey.ADMIRAL2) ) {
                    var secondAdmiral = members[RaceCommandKey.ADMIRAL2];
                    if(secondAdmiral != null ) {
                        return secondAdmiral.has;
                    }
                }
                return false;
            }
        }



        public RaceCommandMember GetCommander() {
            if (hasCommander) {
                return members[RaceCommandKey.COMMANDER];
            }
            return null;
        }

        public RaceCommandMember GetFirstAdmiral() {
            if (hasFirstAdmiral) {
                return members[RaceCommandKey.ADMIRAL1];
            }
            return null;
        }

        public RaceCommandMember GetSecondAdmiral() {
            if (hasSecondAdmiral) {
                return members[RaceCommandKey.ADMIRAL2];
            }
            return null;
        }

        public bool IsCommanderOrAdmiral(string characterID) {
            if (hasCommander) {
                return GetCommander().characterID == characterID;
            }
            if (hasFirstAdmiral) {
                return GetFirstAdmiral().characterID == characterID;
            }
            if (hasSecondAdmiral) {
                return GetSecondAdmiral().characterID == characterID;
            }
            return false;
        }
    }
}
