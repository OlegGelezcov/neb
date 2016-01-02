using Common;
using Nebula.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components {
    public abstract class TimedEffect : IInfo {

        public abstract TimedEffectType type { get; }

        private int m_ExpireTime;
        private float m_Value;

        /// <summary>
        /// When effect will be expired
        /// </summary>
        public int expireTime {
            get {
                return m_ExpireTime;
            }
        }

        /// <summary>
        /// Value of effect
        /// </summary>
        public float value {
            get {
                return m_Value;
            }
        }

        public TimedEffect(float inValue, int inExpireTime  ) {
            Initialize(inValue, inExpireTime);
        }

        protected void Initialize(float inValue, int inExpireTime ) {
            m_ExpireTime = inExpireTime;
            m_Value = inValue;
        }

        public abstract bool CheckExpire(NebulaObject contextObject);
        public abstract Hashtable GetInfo();

        public abstract void ParseInfo(Hashtable info);
    }
}
