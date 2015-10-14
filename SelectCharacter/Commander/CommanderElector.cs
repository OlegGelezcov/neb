using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Commander {
    public class CommanderElector {

        public ObjectId Id { get; set; }

        public int race { get; set; }

        public string login { get; set; }
        public string gameRefID { get; set; }
        public string characterID { get; set; }
        public string candidateCharacterID { get; set; }
    }
}
