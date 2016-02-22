using System.Xml.Linq;

namespace Nebula.Contracts {
    public class ItemDeliveryDataCollection : BaseContractElementCollection<ItemDeliveryElementData> {

        public ItemDeliveryDataCollection(XElement parent)
            : base(parent, "item") { }

        public ItemDeliveryDataCollection() { }

      
    }
}
