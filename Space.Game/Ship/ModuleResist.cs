using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Ship {
    public class ModuleResist {
        private float m_CommonResist;
        private float m_RocketResist;
        private float m_AcidResist;
        private float m_LaserResist;

        public ModuleResist() {
            m_CommonResist = 0.0f;
            m_RocketResist = 0.0f;
            m_AcidResist = 0.0f;
            m_LaserResist = 0.0f;
        }

        public void SetCommonResist(float val) {
            m_CommonResist = val;
        }
        public void SetRocketResist(float val) {
            m_RocketResist = val;
        }
        public void SetAcidResist(float val) {
            m_AcidResist = val;
        }
        public void SetLaserResist(float val) {
            m_LaserResist = val;
        }

        public float commonResist {
            get {
                return m_CommonResist;
            }
        }
        public float rocketResist {
            get {
                return m_RocketResist;
            }
        }
        public float acidResist {
            get {
                return m_AcidResist;
            }
        }
        public float laserResist {
            get {
                return m_LaserResist;
            }
        }
    }
}
