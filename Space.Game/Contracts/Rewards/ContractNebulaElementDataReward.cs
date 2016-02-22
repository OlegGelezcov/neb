using Common;
using System.Xml.Linq;

namespace Nebula.Contracts.Rewards {
    public class ContractNebulaElementDataReward : ContractDataReward {
        public int count { get; private set; }

        public ContractNebulaElementDataReward(XElement element) : base(element) {
            count = element.GetInt("count");
        }
    }
}
