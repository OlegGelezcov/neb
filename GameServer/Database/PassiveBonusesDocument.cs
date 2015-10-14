using MongoDB.Bson;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class PassiveBonusesDocument {
        public ObjectId Id { get; set; }
        public string characterID { get; set; }
        public Dictionary<int, PassiveBonusDbData> bonuses { get; set; }

        public void Set(Dictionary<int, PassiveBonusDbData>  saveData) {
            bonuses = saveData;
            if(bonuses == null ) {
                bonuses = new Dictionary<int, PassiveBonusDbData>();
            }
        }

        public Dictionary<int, PassiveBonusDbData> Get() {
            if(bonuses == null ) {
                bonuses = new Dictionary<int, PassiveBonusDbData>();
            }
            return bonuses;
        }
    }

    public class PassiveBonusDbData {

        //current bonus tier
        public int tier { get; set; }

        //currently learning started
        public bool learningStarted { get; set; }

        //learn sdtart time from 1970
        public int learnStartTime { get; set; }
        //learn end time from 1970
        public int learnEndTime { get; set; }
    } 
}
