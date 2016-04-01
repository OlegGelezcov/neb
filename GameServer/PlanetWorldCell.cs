using Nebula.Engine;
using Nebula.Resources.Zones.Planets;

namespace Nebula {

    public class PlanetWorldCell {
        private PlanetCellData m_Data;
        private NebulaObject m_CellObject;

        public PlanetWorldCell() { }

        public void SetData(PlanetCellData d) {
            m_Data = d;
        }

        public void SetObject(NebulaObject obj) {
            m_CellObject = obj;
        }

        public PlanetCellData data {
            get {
                return m_Data;
            }
        }

        public NebulaObject cellObject {
            get {
                return m_CellObject;
            }
        }

        public bool hasCellObject {
            get {
                return cellObject;
            }
        }

    }
}
