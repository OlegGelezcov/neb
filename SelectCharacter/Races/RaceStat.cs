using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace SelectCharacter.Races {

    /// <summary>
    /// Hold single race stats info
    /// </summary>
    public class RaceStat : IInfoSource {
        public int race { get; set; }
        public int points { get; set; }

        public void Clear() {
            points = 0;
        }

        public void SetPoints(int pt) {
            points = pt;
        }

        public void AddPoints(int pt) {
            points += pt;
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Race, race },
                { (int)SPC.Points, points }
            };
        }
    }
}
