using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Quests.Markers {
    public class PointMarker : StageMarker {
        private Vector3 m_Position;
        private string m_WorldId;

        public Vector3 position {
            get {
                return m_Position;
            }
        }

        public string world {
            get {
                return m_WorldId;
            }
        }

        public PointMarker(string worldId, Vector3 pos)
            : base(MarkerType.point) {
            m_WorldId = worldId;
            m_Position = pos;
        }


    }
}
