using Common;
using System;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public abstract class ContractData {
        public string id { get; private set; }
        public ContractCategory category { get; private set; }
        public int minLevel { get; private set; }
        private ContractDataRewardCollection m_Rewards;

        public ContractData(XElement element ) {
            id = element.GetString("id");
            category = (ContractCategory)Enum.Parse(typeof(ContractCategory), element.GetString("category"));
            minLevel = element.GetInt("min_level");

            var rewardsElement = element.Element("rewards");
            m_Rewards = new ContractDataRewardCollection(rewardsElement);
        }

        public ContractDataRewardCollection rewards {
            get {
                return m_Rewards;
            }
        }

    }
}
