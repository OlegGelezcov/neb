using MongoDB.Bson;
using Nebula.Pets;
using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class PetDocument {
        public ObjectId Id { get; set; }
        public string characterId { get; set; }
        public List<PetSave> pets { get; set; }
        public bool isNewDocument { get; set; }

        public void Set(List<PetSave> saves ) {
            pets = saves;
            isNewDocument = false;
        }

        public PetCollection SourceObject(IRes resource) {
            if(pets == null ) {
                pets = new List<PetSave>();
            }
            return new PetCollection(pets);
        }

    }
}
