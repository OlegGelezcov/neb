using MongoDB.Bson;
using Space.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class TimedEffectsDocument {
        public ObjectId Id { get; set; }
        public Hashtable effects { get; set; }
        public string characterId { get; set; }
        public bool isNewDocument { get; set; }

        public void Set(Hashtable save) {
            effects = save;
            isNewDocument = false;
        }

        public Hashtable SourceObject(IRes resource) {
            if(effects == null ) {
                effects = new Hashtable();
            }
            return effects;
        }
    }
}
