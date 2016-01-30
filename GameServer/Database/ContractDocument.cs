using MongoDB.Bson;
using Nebula.Game.Contracts;
using Space.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class ContractDocument {
        public ObjectId Id { get; set; }
        public string characterId { get; set; }
        public List<Hashtable> completedContracts { get; set; }
        public List<Hashtable> activeContracts { get; set; }
        public bool isNewDocument { get; set; }

        public void Set(ContractSave sourceObject) {
            completedContracts = sourceObject.completedContracts;
            activeContracts = sourceObject.activeContracts;
            isNewDocument = false;
        }

        public ContractSave SourceObject(IRes resource) {
            if(completedContracts == null ) {
                completedContracts = new List<Hashtable>();
            }
            if(activeContracts == null ) {
                activeContracts = new List<Hashtable>();
            }
            return new ContractSave(completedContracts, activeContracts);
        }

    }
}
