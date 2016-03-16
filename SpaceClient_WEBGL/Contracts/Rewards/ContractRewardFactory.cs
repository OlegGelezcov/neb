using Common;
using Nebula.Client.Contracts.Rewards;

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
                case ContractRewardType.scheme:
                    return new SchemeContractReward(element);
                case ContractRewardType.nebula_element:
                    return new NebulaElementContractReward(element);
                case ContractRewardType.craft_resource:
                    return new CraftResourceContractReward(element);
                case ContractRewardType.turret:
                    return new TurretContractReward(element);
                default:
                    return null;
            }
        }
    }
}
