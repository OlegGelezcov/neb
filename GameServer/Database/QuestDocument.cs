using MongoDB.Bson;
using Space.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class QuestDocument {
        public ObjectId Id { get; set; }
        public string characterId { get; set; }
        public Hashtable questHash { get; set; }
        public bool isNewDocument { get; set; }

        public void Set(Hashtable qHash ) {
            questHash = qHash;
            isNewDocument = false;
        }

        public Hashtable SourceObject(IRes resource ) {
            if(questHash == null ) {
                questHash = new Hashtable();
            }
            return questHash;
        }
    }
}
