using System;
using System.Collections;
using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using ServerClientCommon;
using Nebula.Game.Utils;

namespace Nebula.Game.Components {
    public class ExpTimedEffect : TimedEffect {

        //public const int EXP_EFFECT_50 = 1;
        //public const int EXP_EFFECT_100 = 2;
        //public const int EXP_EFFECT_200 = 3;

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        //differ 50% exp effect, from 100% , from 200 %
        private int m_EffectTag;
        
        public ExpTimedEffect() : this(0, 0, 0) { }

        public ExpTimedEffect(float val, int expire, int tag) 
            : base(val, expire) {
            m_EffectTag = tag;
        }

        public override TimedEffectType type {
            get {
                return TimedEffectType.exp;
            }
        }

        public override bool CheckExpire(NebulaObject contextObject) {

            var bonuses = contextObject.Bonuses();

            if (CommonUtils.SecondsFrom1970() > expireTime) {
                bonuses.RemoveBuffs(BonusType.increase_exp_on_pc);
                return true;
            } else {
                
                var result = bonuses.Contains(BonusType.increase_exp_on_pc, "ExpTimedEffect", m_EffectTag);

                switch (result) {
                    case BuffSearchResult.NotContains: {
                            //s_Log.InfoFormat("Not contains buff: create new".Color(LogColor.orange));
                            RecreateBonus(bonuses);
                        }
                        break;
                    case BuffSearchResult.ContainsWithDifferentTag: {
                            //s_Log.InfoFormat("Contains buff with different tag: remove old and create new buff".Color(LogColor.orange));
                            bonuses.RemoveBuffs(BonusType.increase_exp_on_pc);
                            RecreateBonus(bonuses);
                        }
                        break;
                    case BuffSearchResult.Contains: {
                            //s_Log.InfoFormat("already contains same buff: do nothing".Color(LogColor.orange));
                        }
                        break;
                }
                return false;
            }
        }

        public override Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Expire, expireTime },
                { (int)SPC.Value, value },
                { (int)SPC.Tag, m_EffectTag }
            };
        }

        public override void ParseInfo(Hashtable info) {
            int expire = info.GetValue<int>((int)SPC.Expire, 0);
            float val = info.GetValue<float>((int)SPC.Value, 0f);
            int tag = info.GetValue<int>((int)SPC.Tag, 0);
            base.Initialize(val, expire);
            m_EffectTag = tag;
        }

        private void RecreateBonus(PlayerBonuses target ) {
            float interval = expireTime - CommonUtils.SecondsFrom1970();
            if(interval < 0f ) {
                interval = 0f;
            }

            Buff buff = new Buff("ExpTimedEffect", null, Common.BonusType.increase_exp_on_pc, interval, value);
            buff.SetTag(m_EffectTag);
            target.SetBuff(buff);
        }

        public override string ToString() {
            return string.Format("type = {0}, value = {1}, tag = {2}, expire = {3}", type, value, m_EffectTag, expireTime);
        }
    }
}
