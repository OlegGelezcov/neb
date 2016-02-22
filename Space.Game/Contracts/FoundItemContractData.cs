using Common;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class FoundItemContractData : ContractData  {
        private FoundItemDataCollection m_Items;

        public FoundItemContractData(XElement element)
            : base(element) {
            var itemsElement = element.Element("items");
            m_Items = new FoundItemDataCollection(itemsElement);
        }

        public int GetCount(Race race) {
            return m_Items.GetCount(race);
        }
        public int GetCount(Race race, int level) {
            return m_Items.GetCount(race, level);
        }
        public bool Has(Race race, int level) {
            return m_Items.GetCount(race, level) > 0;
        }
        public FoundItemData GetRandom(Race race, int level ) {
            return m_Items.GetRandom(race, level);
        }
    }
}
