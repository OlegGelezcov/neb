using Common;

#if UP
using Nebula.Client.UP;
#else
using Nebula.Client.UP;
using System.Xml.Linq;
#endif
namespace Nebula.Client.Contracts {
    public abstract class ContractReward {
        public ContractRewardType rewardType { get; private set; }

        public ContractReward(UPXElement element) {
            rewardType = (ContractRewardType)System.Enum.Parse(typeof(ContractRewardType), element.GetString("type"));
        }

        public ContractReward(XElement element) {
            rewardType = (ContractRewardType)System.Enum.Parse(typeof(ContractRewardType), element.GetString("type"));
        }
    }
}
