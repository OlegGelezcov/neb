using System.Xml.Linq;

namespace Nebula.Resources.PlayerConstructions {
    public class ConstructionDataResource {
        private TurretConstructionData m_TurretConstructionData;
        private FortificationConstructionData m_FortificationConstructionData;
        private OutpostConstructionData m_OutpostConstructionData;
        private MiningStationConstructionData m_MiningStationComponentData;

        public PlanetCommandCenterData planetCommandCenterData { get; private set; }
        public PlanetTurretData planetTurretData { get; private set;  }
        public PlanetMiningStationData planetMiningStationData { get; private set; }
        public PlanetResourceHangarData planetResourceHangarData { get; private set; }
        public PlanetResourceAcceleratorData planetResourceAcceleratorData { get; private set; }

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

            planetCommandCenterData = new PlanetCommandCenterData(document.Element("constructions").Element("planet_command_center"));
            planetTurretData = new PlanetTurretData(document.Element("constructions").Element("planet_turret"));
            planetMiningStationData = new PlanetMiningStationData(document.Element("constructions").Element("planet_mining_station"));
            planetResourceHangarData = new PlanetResourceHangarData(document.Element("constructions").Element("planet_resource_hangar"));
            planetResourceAcceleratorData = new PlanetResourceAcceleratorData(document.Element("constructions").Element("planet_resource_accelerator"));

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
