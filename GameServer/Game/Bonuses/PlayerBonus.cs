using Common;
using ExitGames.Logging;
using Nebula.Game.Components;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nebula.Game.Bonuses {
    public class PlayerBonus
    {
        public BonusType type { get; private set; }
        public ConcurrentDictionary<string, Buff> buffs { get; private set; }

        private List<string> mInvalidBuffs = new List<string>();

        private static ILogger log = LogManager.GetCurrentClassLogger();

        public PlayerBonus(BonusType type)
        {
            this.type = type;
            this.buffs = new ConcurrentDictionary<string, Buff>();
        }

        public int count {
            get {
                return buffs.Count;
            }
        }

        public bool hasAny {
            get {
                return count > 0;
            }
        }

        public ConcurrentBag<PlayerBonuses.BuffInfo> GetBuffInfoCollection() {
            ConcurrentBag<PlayerBonuses.BuffInfo> rbuffs = new ConcurrentBag<PlayerBonuses.BuffInfo>();
            foreach(var pBuff in buffs) {
                if (pBuff.Value.interval > 0) {
                    rbuffs.Add(new PlayerBonuses.BuffInfo { bonusType = type, time = pBuff.Value.interval, value = pBuff.Value.value });
                }
            }
            return rbuffs;
        } 

        public Buff any {
            get {
                foreach(var buffPair in buffs) {
                    return buffPair.Value;
                }
                return null;
            }
        }

        public void RemoveAny() {
            if(buffs.Count > 0 ) {

            }
            string key = string.Empty;
            foreach(var pBuff in buffs) {
                key = pBuff.Key;
                break;
            }
            if(!string.IsNullOrEmpty(key)) {
                Buff removedBuff = null;
                if (buffs.TryRemove(key, out removedBuff)) {
                    log.InfoFormat("PlayerBonus.RemoveAny(): removed {0}:{1} [yellow]", removedBuff.buffType, removedBuff.id);
                }
            }
        }

        public int GetBuffCountWithTag(int tag) {
            int counter = 0;
            foreach(var pbuff in buffs) {
                if(pbuff.Value.tag == tag) {
                    counter++;
                }
            }
            return counter;
        }

        public Buff GetBuff(string id) {
            Buff result = null;
            buffs.TryGetValue(id, out result);
            return result;
        }

        public void MultInterval(float mult) {
            foreach(var p in buffs) {
                p.Value.MultInterval(mult);
            }
        }

        public float value {
            get {
                float total = 0f;
                foreach(var buffPair in buffs) {
                    total += buffPair.Value.value;
                }
                return total;
            }
        }

        public void ScaleBuffInterval(float mult) {
            if(buffs == null ) { return; }
            foreach (var buffPair in buffs) {
                buffPair.Value.ScaleInterval(mult);
            }
        }
      
        public void Update(float deltaTime) {
            mInvalidBuffs.Clear();
            foreach(var buffPair in buffs) {
                buffPair.Value.Update(deltaTime);
                if(!buffPair.Value.valid) {
                    mInvalidBuffs.Add(buffPair.Key);
                }
            }

            foreach(string invalidBuffId in mInvalidBuffs) {
                Buff removedValue = null;
                buffs.TryRemove(invalidBuffId, out removedValue);
            }
        }

        public void SetBuff(Buff buff) {
            buffs[buff.id] = buff;
        }

        public void SetBuffValue(string buffId, float value) {
            Buff buff = null;
            if(buffs.TryGetValue(buffId, out buff)) {
                buff.SetValue(value);
            }
        }

        public void RemoveBuff(string buffId) {
            Buff removedBuff = null;
            buffs.TryRemove(buffId, out removedBuff);
        }



        public bool Contains(string buffId) {
            return buffs.ContainsKey(buffId);
        } 

        public bool hasBuffs {
            get {
                return buffs.Count > 0;
            }
        }

        public void Clear() {
            buffs.Clear();
        }
    }
}
