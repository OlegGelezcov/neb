using System;
using Nebula.Engine;
using Space.Game;

namespace Nebula.Game.Pets.Conditions {
    public class CooldownCondition : Condition {

        private float m_Cooldown;
        private float m_Time;

        public CooldownCondition(float cooldown, NebulaObject source) : base(source) {
            m_Cooldown = cooldown;
        }

        public override bool Check() {
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
