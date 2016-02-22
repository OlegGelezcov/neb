using Nebula.Client.UP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Contracts.Rewards {
    public class NebulaElementContractReward : ContractReward {
        public int count { get; private set; }

        public NebulaElementContractReward(UniXMLElement element)
            : base(element) {
            count = element.GetInt("count");
        }
    }
}
