using MongoDB.Bson;
using Nebula.Game.Components;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Database {
    public class AchievmentDocument {
        public ObjectId Id { get; set; }
        public string characterId { get; set; }
        public Hashtable variables { get; set; }
        public List<string> visitedZones { get; set; }
        public bool isNewDocument { get; set; }
        public int points { get; set; }

        public void Set(AchievmentSave save) {
            variables = save.variables;
            visitedZones = save.visitedZones;
            points = save.points;
            isNewDocument = false;
        }

        public AchievmentSave SourceObject() {
            if(variables == null ) {
                variables = new Hashtable();
            }
            if(visitedZones == null ) {
                visitedZones = new List<string>();
            }
            return new AchievmentSave(variables, visitedZones, points);
        }
    }
}
