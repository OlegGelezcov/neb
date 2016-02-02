using Common;
using System;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public abstract class ContractDataReward {
        public ContractRewardType type { get; private set; }

        public ContractDataReward(XElement element ) {
            type = (ContractRewardType)Enum.Parse(typeof(ContractRewardType), element.GetString("type"));
        }
    }
}
