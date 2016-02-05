using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class KillNPCGroupContractMarkData : ContractMarkData {

        public string group { get; private set; }

        public KillNPCGroupContractMarkData(XElement element)
            : base(element) {
            group = element.GetString("group");
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.kill_npc_group_contract_mark;
            }
        }
    }
}
