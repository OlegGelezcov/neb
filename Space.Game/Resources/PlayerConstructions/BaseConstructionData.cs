using Common;
using System.Xml.Linq;

namespace Nebula.Resources.PlayerConstructions {
    public class BaseConstructionData {

        private float m_Size;
        private float m_Hp;
        private bool m_IgnoreDamageAtStart;
        private float m_IgnoreDamageInterval;
        private bool m_CreateContainer;

        public BaseConstructionData(XElement element) {
            m_Size = element.GetFloat("size");
            m_Hp = element.GetFloat("max_hp");
            m_IgnoreDamageAtStart = element.GetBool("ignore_damage_after_spawn");
            m_IgnoreDamageInterval = element.GetFloat("ignore_damage_interval");
            m_CreateContainer = element.GetBool("create_container");
        }
        
        public float size {
            get {
                return m_Size;
            }
        }

        public float hp {
            get {
                return m_Hp;
            }
        }

        public bool ignoreDamageAtStart {
            get {
                return m_IgnoreDamageAtStart;
            }
        }

        public float ignoreDamageInterval {
            get {
                return m_IgnoreDamageInterval;
            }
        }

        public bool createContainer {
            get {
                return m_CreateContainer; 
            }
        }
    }
}
