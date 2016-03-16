using Common;
using System.Xml.Linq;

namespace Nebula.Contracts.Rewards {
    public class ContractTurretDataReward : ContractDataReward {
        public int count { get; private set; }

        public ContractTurretDataReward(XElement element) 
            : base(element) {
            count = element.GetInt("count");
        }
    }
}
