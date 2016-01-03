using System;
using Nebula.Engine;
using Space.Game;

namespace Nebula.Game.Pets {
    public class CooldownCondition : Condition {

        private float m_Cooldown;
        private float m_Time;

        public CooldownCondition(float cooldown) {
            m_Cooldown = cooldown;
        }

        public override bool Check(NebulaObject source) {
            if(m_Time + m_Cooldown <= Time.curtime() ) {
                return true;
            }
            return false;
        }

        public override void Renew() {
            m_Time = Time.curtime();
        }
    }
}
