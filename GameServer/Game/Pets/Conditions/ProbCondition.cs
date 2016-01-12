using System;
using GameMath;
using Nebula.Engine;

namespace Nebula.Game.Pets.Conditions {
    public class ProbCondition : Condition {
        private float m_Prob;

        public ProbCondition(float prob, NebulaObject source) : base(source) {
            m_Prob = prob;
        }

        public override bool Check() {
            if(Rand.Float01() < m_Prob ) {
                return true;
            }
            return false;
        }

        public override void Renew() {
        }
    }
}
