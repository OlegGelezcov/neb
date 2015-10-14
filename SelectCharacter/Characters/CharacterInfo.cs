using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Characters {
    public class CharacterInfo {
        public ObjectId Id { get; set; }
        public string characterID { get; set; } = string.Empty;
        public string characterName { get; set; } = string.Empty;
    }
}
