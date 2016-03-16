using Common;
using Nebula.Contracts.Rewards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class ContractDataRewardCollection {

        private List<ContractDataReward> m_Rewards;

        public ContractDataRewardCollection(XElement element) {
            var elements = element.Elements("reward").ToList();
            m_Rewards = new List<ContractDataReward>();
            foreach(var elem in elements) {
                var reward = Create(elem);
                if(reward != null ) {
                    m_Rewards.Add(reward);
                }
            }
        }

        private ContractDataReward Create(XElement element) {
            ContractRewardType type = (ContractRewardType)Enum.Parse(typeof(ContractRewardType), element.GetString("type"));
            switch(type) {
                case ContractRewardType.credits:
                    return new ContractCreditsDataReward(element);
                case ContractRewardType.exp:
                    return new ContractExpDataReward(element);
                case ContractRewardType.ore:
                    return new ContractOreDataReward(element);
                case ContractRewardType.weapon:
                    return new ContractWeaponDataReward(element);
                case ContractRewardType.scheme:
                    return new ContractSchemeDataReward(element);
                case ContractRewardType.nebula_element:
                    return new ContractNebulaElementDataReward(element);
                case ContractRewardType.craft_resource:
                    return new ContractCraftResourceDataReward(element);
                case ContractRewardType.turret:
                    return new ContractTurretDataReward(element);
                default:
                    return null;
            }
        }

        public List<ContractDataReward> rewards {
            get {
                return m_Rewards;
            }
        }
    }
}
