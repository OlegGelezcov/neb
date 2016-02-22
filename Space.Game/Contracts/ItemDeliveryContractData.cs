using Common;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class ItemDeliveryContractData : ContractData {
        private ItemDeliveryDataCollection m_Elements;

        public ItemDeliveryContractData(XElement element) 
            : base(element) {
            var itemsElement = element.Element("items");
            m_Elements = new ItemDeliveryDataCollection(itemsElement);
        }

        public int GetCount(Race race) {
            return m_Elements.GetCount(race);
        }
        public int GetCount(Race race, int level) {
            return m_Elements.GetCount(race, level);
        }
        public bool Has(Race race, int level) {
            return m_Elements.GetCount(race, level) > 0;
        }
        public ItemDeliveryElementData GetRandom(Race race, int level) {
            return m_Elements.GetRandom(race, level);
        }

         
    }
}
