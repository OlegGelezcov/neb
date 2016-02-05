using Common;

namespace Nebula.Client.Contracts {
    public class ContractRewardFactory {

        public ContractReward Create(UniXMLElement element) {
            ContractRewardType rewardType = (ContractRewardType)System.Enum.Parse(typeof(ContractRewardType), element.element.GetString("type"));
            switch(rewardType) {
                case ContractRewardType.credits:
                    return new CreditsContractReward(element.element);
                case ContractRewardType.exp:
                    return new ExpContractReward(element.element);
                case ContractRewardType.ore:
                    return new OreContractReward(element.element);
                case ContractRewardType.weapon:
                    return new WeaponContractReward(element.element);
                default:
                    return null;
            }
        }
    }
}
