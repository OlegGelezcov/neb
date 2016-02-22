using Common;
using System.Xml.Linq;

namespace Nebula.Contracts.Rewards {
    public class ContractCraftResourceDataReward : ContractDataReward {
        public int count { get; private set; }
        public ContractCraftResourceDataReward(XElement element) : base(element) {
            count = element.GetInt("count");
        }
    }
}
