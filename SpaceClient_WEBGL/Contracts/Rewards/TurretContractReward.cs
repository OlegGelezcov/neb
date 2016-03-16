using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Contracts.Rewards {
    public class TurretContractReward : ContractReward {
        public int count { get; private set; }
        public TurretContractReward(UniXMLElement element)
            : base(element) {
            count = element.GetInt("count");
        }

    }
}
