using Common;
using System.Xml.Linq;

namespace Nebula.Resources.PlayerConstructions {
    public class TurretConstructionData : AttackerDefensiveConstructionData  {

        private float m_WanderRadius;

        public TurretConstructionData(XElement element)
            : base(element) {
            m_WanderRadius = element.GetFloat("wander_radius");
        }

        public float wanderRadius {
            get {
                return m_WanderRadius;
            }
        }
    }
}
