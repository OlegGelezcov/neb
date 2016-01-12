using GameMath;
using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components {
    public struct InputHeal {

        private float m_Val;
        private NebulaObject m_Source;
        private bool m_Primary;

        public InputHeal(float val, NebulaObject source = null, bool primary = true) {
            m_Source = source;
            m_Val = Mathf.Abs(val);
            m_Primary = true;
        }

        public NebulaObject source {
            get {
                return m_Source;
            }
        }

        public float value {
            get {
                return m_Val;
            }
        }

        public bool primary {
            get {
                return m_Primary;
            }
        }

        public bool hasSource {
            get {
                return source != null;
            }
        }
    }
}
