using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class ContractDropListComponentData : DropListComponentData {
        public ContractDropListComponentData(XElement element)
            : base(element) { }
        public override ComponentSubType subType {
            get {
                return ComponentSubType.contract_drop_list;
            }
        }
    }
}
