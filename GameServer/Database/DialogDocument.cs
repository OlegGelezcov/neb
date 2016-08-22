using MongoDB.Bson;
using Space.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class DialogDocument {
        public ObjectId Id { get; set; }
        public string characterId { get; set; }
        public Hashtable dialogHash { get; set; }
        public bool isNewDocument { get; set; }

        public void Set(Hashtable dHash ) {
            dialogHash = dHash;
            isNewDocument = false;
        }

        public Hashtable SourceObject(IRes resource) {
            if(dialogHash == null ) {
                dialogHash = new Hashtable();
            }
            return dialogHash;
        }
    }
}
