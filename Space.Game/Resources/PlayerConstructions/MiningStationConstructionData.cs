using Common;
using System.Xml.Linq;

namespace Nebula.Resources.PlayerConstructions {
    public class MiningStationConstructionData : BaseConstructionData {

        private bool m_UseHitProbForAgro;
        private float m_OptimalDistance;
        private float m_Cooldown;
        private float m_DamageInTargetHpPc;

        public MiningStationConstructionData(XElement element) 
            : base(element) {
            m_UseHitProbForAgro = element.GetBool("use_hit_prob_for_agro");
            m_OptimalDistance = element.GetFloat("optimal_distance");
            m_Cooldown = element.GetFloat("cooldown");
            m_DamageInTargetHpPc = element.GetFloat("damage_in_target_hp");
        }

        public bool useHitProbForAgro {
            get {
                return m_UseHitProbForAgro;
            }
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

        public float damageInTargetHp {
            get {
                return m_DamageInTargetHpPc;
            }
        }
    }
}
