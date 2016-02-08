using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public abstract class ContractMarkData : MultiComponentData {

        public string contractId { get; private set; }

        public ContractMarkData(XElement element ) {
            contractId = element.GetString("contract_id");
        }

        public override ComponentID componentID {
            get {
                return ComponentID.ContractMark;
            }
        }
    }
}
