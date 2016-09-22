using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Quests.Markers {
    public class StageMarker {
        private MarkerType m_Type;

        public MarkerType type {
            get {
                return m_Type;
            }
        }

        public StageMarker(MarkerType type) {
            m_Type = type;
        }
    }
}
