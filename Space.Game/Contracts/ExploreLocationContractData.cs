using Common;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class ExploreLocationContractData : ContractData  {
        private LocationDataCollection m_Locations;

        public ExploreLocationContractData(XElement element) 
            : base(element) {
            var locationsElement = element.Element("locations");
            m_Locations = new LocationDataCollection(locationsElement);
        }

        public int GetCount(Race race) {
            return m_Locations.GetCount(race);
        }
        public int GetCount(Race race, int level) {
            return m_Locations.GetCount(race, level);
        }
        public bool Has(Race race, int level) {
            return m_Locations.GetCount(race, level) > 0;
        }
        public LocationData GetRandom(Race race, int level) {
            return m_Locations.GetRandom(race, level);
        }
    }
}
