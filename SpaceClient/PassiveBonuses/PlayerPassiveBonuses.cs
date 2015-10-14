using Common;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Client.PassiveBonuses {
    public class PlayerPassiveBonuses : IInfoParser {
        public Dictionary<PassiveBonusType, PassiveBonusInfo> bonuses { get; private set; }

        public PlayerPassiveBonuses(Hashtable info) {
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            bonuses = new Dictionary<PassiveBonusType, PassiveBonusInfo>();
            foreach(DictionaryEntry entry in info ) {
                PassiveBonusType type = (PassiveBonusType)(int)entry.Key;
                Hashtable bonusInfo = entry.Value as Hashtable;
                if(bonusInfo != null ) {
                    bonuses.Add(type, new PassiveBonusInfo(bonusInfo));
                }
            }
        }
    }
}
