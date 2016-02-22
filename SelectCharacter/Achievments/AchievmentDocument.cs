using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Achievments {
    public class AchievmentDocument {
        public ObjectId Id { get; set; }
        public string characterId { get; set; }
        public Hashtable variables { get; set; }
        public List<string> visitedZones { get; set; }
        public bool isNewDocument { get; set; }
    }
}
