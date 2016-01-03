using System;
using GameMath;
using Nebula.Engine;

namespace Nebula.Game.Pets {
    public class ProbCondition : Condition {
        private float m_Prob;

        public ProbCondition(float prob) {
            m_Prob = prob;
        }

        public override bool Check(NebulaObject source) {
            if(Rand.Float01() < m_Prob ) {
                return true;
            }
            return false;
        }

        public override void Renew() {
        }
    }
}
