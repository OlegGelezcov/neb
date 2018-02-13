using MongoDB.Bson;
using Nebula.Game.Components.Quests;
using Space.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    /*
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
    }*/

    public class QuestDocument {
        public ObjectId Id { get; set; }
        public string characterId { get; set; }
        public List<string> CompletedQuests { get; set; }
        public List<Hashtable> StartedQuests { get; set; }
        public Hashtable QuestVariables { get; set; }

        public bool isNewDocument { get; set; }

        public void Set(QuestSave save) {
            CompletedQuests = save.CompletedQuests;
            StartedQuests = save.StartedQuests;
            QuestVariables = save.QuestVariables;
            CheckNotNullVariables();
            isNewDocument = false;
        }

        private void CheckNotNullVariables() {
            if (CompletedQuests == null) {
                CompletedQuests = new List<string>();
            }
            if (StartedQuests == null) {
                StartedQuests = new List<Hashtable>();
            }
            if(QuestVariables == null ) {
                QuestVariables = new Hashtable();
            }
        }

        public QuestSave SourceObject() {
            CheckNotNullVariables();
            return new QuestSave(CompletedQuests, StartedQuests, QuestVariables);
        }
    }
}
