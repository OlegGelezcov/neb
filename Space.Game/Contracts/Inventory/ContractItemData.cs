using Common;
using System.Xml.Linq;

namespace Nebula.Contracts.Inventory {
    /// <summary>
    /// Inventory item data for contract items
    /// </summary>
    public class ContractItemData {
        public string id { get; private set; }

        public ContractItemData(XElement element) {
            id = element.GetString("id");
        }

    }
}
