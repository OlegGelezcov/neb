using System.Xml.Linq;

namespace Nebula.Resources.PlayerConstructions {
    public class ConstructionDataResource {
        private TurretConstructionData m_TurretConstructionData;
        private FortificationConstructionData m_FortificationConstructionData;
        private OutpostConstructionData m_OutpostConstructionData;
        private MiningStationConstructionData m_MiningStationComponentData;

        public ConstructionDataResource() {

        }

        public void Load(string file) {
            XDocument document = XDocument.Load(file);

            var turretElemet = document.Element("constructions").Element("turret");
            m_TurretConstructionData = new TurretConstructionData(turretElemet);

            var fortificationsElement = document.Element("constructions").Element("fortification");
            m_FortificationConstructionData = new FortificationConstructionData(fortificationsElement);

            var outpostElement = document.Element("constructions").Element("outpost");
            m_OutpostConstructionData = new OutpostConstructionData(outpostElement);

            var miningStationElement = document.Element("constructions").Element("mining_station");
            m_MiningStationComponentData = new MiningStationConstructionData(miningStationElement);
        }

        public TurretConstructionData turret {
            get {
                return m_TurretConstructionData;
            }
        }

        public FortificationConstructionData fortification {
            get {
                return m_FortificationConstructionData;
            }
        }

        public OutpostConstructionData outpost {
            get {
                return m_OutpostConstructionData;
            }
        }

        public MiningStationConstructionData miningStation {
            get {
                return m_MiningStationComponentData;
            }
        }
    }
}
