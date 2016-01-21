using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources.PlayerConstructions {
    public abstract class DefensiveConstructionData : BaseConstructionData {

        private float m_SeparateDistance;
        private float m_FixedInputDamage;
        private float m_AdditionalHp;

        public DefensiveConstructionData(XElement element)
            : base(element) {
            m_SeparateDistance = element.GetFloat("separate_distance");
            m_FixedInputDamage = element.GetFloat("fixed_input_damage");
            m_AdditionalHp = element.GetFloat("additional_hp");
        }

        public float separateDistance {
            get {
                return m_SeparateDistance;
            }
        }

        public float fixedInputDamage {
            get {
                return m_FixedInputDamage;
            }
        }

        public float additionalHp {
            get {
                return m_AdditionalHp;
            }
        }
    }
}
