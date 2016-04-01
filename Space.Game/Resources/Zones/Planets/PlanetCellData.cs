using Common;
using GameMath;
using System.Xml.Linq;

namespace Nebula.Resources.Zones.Planets {
    public class PlanetCellData {
        private int m_Row;
        private int m_Column;
        private Vector3 m_Position;

        public PlanetCellData(XElement element) {
            m_Row = element.GetInt("row");
            m_Column = element.GetInt("column");
            m_Position = element.GetFloatArray("position").ToVector3();
        } 

        public int row {
            get {
                return m_Row;
            }
        }

        public int column {
            get {
                return m_Column;
            }
        }

        public Vector3 position {
            get {
                return m_Position;
            }
        }
    }
}
