using Common;
using System.Xml.Linq;

namespace Nebula.Resources.PlayerConstructions {
    public abstract  class AttackerDefensiveConstructionData : DefensiveConstructionData {

        private float m_OptimalDistance;
        private float m_Cooldown;
        private float m_DamageInTargetHpPc;
        private bool m_UseHitProbForAgro;

        public AttackerDefensiveConstructionData(XElement element) 
            : base(element) {
            m_OptimalDistance = element.GetFloat("optimal_distance");
            m_Cooldown = element.GetFloat("cooldown");
            m_DamageInTargetHpPc = element.GetFloat("damage_in_target_hp");
            m_UseHitProbForAgro = element.GetBool("use_hit_prob_for_agro");
        }

        public float optimalDistance {
            get {
                return m_OptimalDistance;
            }
        }

        public float cooldown {
            get {
                return m_Cooldown;
            }
        }

        public float damageInTargetHpPc {
            get {
                return m_DamageInTargetHpPc;
            }
        }

        public bool useHitProbForAgro {
            get {
                return m_UseHitProbForAgro;
            }
        }
    }
}
