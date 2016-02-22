using System.Xml.Linq;

namespace Nebula.Contracts {
    public class FoundItemDataCollection : BaseContractElementCollection<FoundItemData> {

        public FoundItemDataCollection(XElement parent)
            : base(parent, "item") { }

        public FoundItemDataCollection() { }
    }
}
