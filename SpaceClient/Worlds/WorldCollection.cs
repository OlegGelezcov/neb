using Common;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Client.Worlds {
    public class WorldCollection : IInfoParser {
        public Dictionary<string, WorldInfo> worlds { get; private set; }

        public WorldCollection() {
            worlds = new Dictionary<string, WorldInfo>();
        }

        public void ParseInfo(Hashtable info) {
            worlds.Clear();
            foreach(DictionaryEntry entry in info ) {
                worlds.Add(entry.Key.ToString(), new WorldInfo(entry.Value as Hashtable));
            }
        }

        public WorldInfo GetWorld(string ID) {
            if(worlds.ContainsKey(ID)) {
                return worlds[ID];
            }
            return null;
        }

        public WorldInfo GetFreeSourceWorld(Race race) {
            switch(race) {
                case Race.Humans:
                    return GetWorld("H1");
                case Race.Criptizoids:
                    return GetWorld("E1");
                case Race.Borguzands:
                    return GetWorld("B1");
                default:
                    return GetWorld("H1");
            }
            //int minPlayerCount = int.MaxValue;
            //WorldInfo result = null;

            //foreach(var pw in worlds) {
            //    if(pw.Value.worldType == WorldType.source && pw.Value.startRace == race && pw.Value.playerCount < minPlayerCount ) {
            //        minPlayerCount = pw.Value.playerCount;
            //        result = pw.Value;
            //    }
            //}

            //return result;
        }

        public bool IsSourceWorld(string ID, Race race) {
            foreach(var pw in worlds) {
                if(pw.Value.ID == ID && pw.Value.startRace == race) {
                    if(pw.Value.worldType == WorldType.source) {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool UnderAttack(string ID) {
            if(worlds.ContainsKey(ID)) {
                return worlds[ID].underAttack;
            }
            return false;
        }

        public void Clear() {
            worlds.Clear();
        }
    }
}
