using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using GameMath;

namespace Nebula.Client
{
    public class ActorBonuses
    {
        private Dictionary<BonusType, float> bonuses;

        public ActorBonuses() {
            bonuses = new Dictionary<BonusType, float>();
        }

        public ActorBonuses(Hashtable data) {
            bonuses = new Dictionary<BonusType, float>();

            Replace(data);
        }

        public void Replace(Hashtable data) {
            bonuses.Clear();
            foreach (DictionaryEntry entry in data)
            {
                int iKey = -1;
                if(entry.Key.GetType() == typeof(byte)) {
                    iKey = (byte)entry.Key;
                } else if(entry.Key.GetType() == typeof(int)) {
                    iKey = (int)entry.Key;
                }
                if (iKey >= 0) {
                    bonuses.Add((BonusType)(byte)iKey, (float)entry.Value);
                }
            }
        }

        public Dictionary<BonusType, float> Bonuses 
        {
            get {
                return bonuses;
            }
        }

        public float Value(BonusType key)
        {
            if(this.bonuses.ContainsKey(key))
            {
                return this.bonuses[key];
            }
            return 0f;
        }

        public bool IsNon(BonusType key) {
            return Mathf.CompareFloat(Mathf.Abs(Value(key)), 0f);
        }

        public void Clear() {
            bonuses.Clear();
        }
    }
}
