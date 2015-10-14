using System;
using System.Collections;
using Common;
using MongoDB.Bson;

namespace SelectCharacter.Races {
    public class RaceStats : IInfoSource {
        public ObjectId Id { get; set; }

        public RaceStat humans { get; set; } = new RaceStat {  race = (int)(byte)Race.Humans, points = 0};
        public RaceStat borguzands { get; set; } = new RaceStat { race = (int)(byte)Race.Borguzands, points = 0};
        public RaceStat criptizids { get; set; } = new RaceStat { race = (int)(byte)Race.Criptizoids, points = 0 };

        public void Clear() {
            humans.Clear();
            borguzands.Clear();
            criptizids.Clear();
        }

        public void SetPoints(Race race, int pt) {
            switch(race) {
                case Race.Humans: humans.SetPoints(pt); break;
                case Race.Borguzands: borguzands.SetPoints(pt); break;
                case Race.Criptizoids: criptizids.SetPoints(pt); break;
            }
        }

        public void AddPoints(Race race, int pt) {
            switch (race) {
                case Race.Humans: humans.AddPoints(pt); break;
                case Race.Borguzands: borguzands.AddPoints(pt); break;
                case Race.Criptizoids: criptizids.AddPoints(pt); break;
            }
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)(byte)Race.Humans, humans.GetInfo() },
                { (int)(byte)Race.Borguzands, borguzands.GetInfo() },
                { (int)(byte)Race.Criptizoids, criptizids.GetInfo() }
            };
        }
    }
}
