using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Characters {
    public class DatabaseCharacterName {
        public ObjectId Id { get; set; }

        public string characterId { get; set; }
        public string characterName { get; set; }
        public string gameRef { get; set; }
    }
}
