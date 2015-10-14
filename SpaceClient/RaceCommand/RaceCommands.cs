using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Nebula.Client.RaceCommand {
    public class RaceCommands : IInfoParser {
        public Dictionary<Race, RaceCommand> commands { get; private set; }

        public RaceCommands() {
            commands = new Dictionary<Race, RaceCommand>();
        }

        public void ParseInfo(Hashtable info) {
            commands.Clear();

            foreach(DictionaryEntry raceCommandEntry in info) {
                Race race = (Race)(byte)(int)raceCommandEntry.Key;
                Hashtable raceCommandInfo = raceCommandEntry.Value as Hashtable;
                commands.Add(race, new RaceCommand(race, raceCommandInfo));
            }
        }

        public bool HasCommand(Race race) {
            return commands.ContainsKey(race);
        }

        public RaceCommand GetCommand(Race race) {
            if(HasCommand(race)) {
                return commands[race];
            }
            return null;
        }
    }
}
