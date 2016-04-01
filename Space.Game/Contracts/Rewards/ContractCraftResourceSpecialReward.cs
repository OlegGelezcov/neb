using Common;
using System.Xml.Linq;

namespace Nebula.Contracts.Rewards {
    public class ContractCraftResourceSpecialReward : ContractDataReward {
        public int count { get; private set; }
        public string id { get; private set; }

        public ContractCraftResourceSpecialReward(XElement element) 
            : base(element) {
            count = element.GetInt("count");
            id = element.GetString("id");
        }

    }
}
