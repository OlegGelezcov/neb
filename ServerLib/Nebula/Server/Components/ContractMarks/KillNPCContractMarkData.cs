using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class KillNPCContractMarkData : ContractMarkData {

        public string group { get; private set; }

        public KillNPCContractMarkData(XElement element) 
            : base(element) {
            group = element.GetString("group");
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.kill_special_npc_contract_mark;
            }
        }
    }
}
