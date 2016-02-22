using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class FoundItemContractMarkData : ContractMarkData {
        public string group { get; private set; }

        public FoundItemContractMarkData(XElement element) : base(element) {
            group = element.GetString("group");
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.found_item_contract_mark;
            }
        }
    }
}
