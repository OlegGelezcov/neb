using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Commander {
    public class RaceCommandCache {

        private ConcurrentDictionary<int, RaceCommand> mCommands = new ConcurrentDictionary<int, RaceCommand>();
        public bool changed { get; private set; } = false;


        public RaceCommand GetCommand(int race) {
            RaceCommand command = null;
            if(mCommands.TryGetValue(race, out command)) {
                return command;
            }
            return null;
        }

        public void SetChanged(bool changed) {
            this.changed = changed;
        }

        public void Save(MongoCollection<RaceCommand> collection ) {
            if(changed) {
                foreach(var p in mCommands) {
                    collection.Save(p.Value);
                }
                SetChanged(false);
            }
        }

        public void AddCommand(RaceCommand command) {
            mCommands.TryAdd(command.race, command);
        }
    }
}
