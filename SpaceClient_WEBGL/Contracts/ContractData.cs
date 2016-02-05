using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Contracts {
    public class ContractData {
        public string id { get; private set; }
        public ContractCategory category { get; private set; }
        public string name { get; private set; }
        public string acceptText { get; private set; }
        public string hintText { get; private set; }
        public string completeText { get; private set; }
        public int minLevel { get; private set; }
        public List<ContractReward> rewards { get; private set; }

        public ContractData(UniXMLElement element) {
            id = element.element.GetString("id");
            category = (ContractCategory)System.Enum.Parse(typeof(ContractCategory), element.element.GetString("category"));
            name = element.element.GetString("name");
            acceptText = element.element.GetString("accept_text");
            hintText = element.element.GetString("hint");
            completeText = element.element.GetString("complete_text");
            minLevel = element.element.GetInt("min_level");

            var rewardElements = element.element.Element("rewards").Elements("reward").ToList();
            rewards = new List<ContractReward>();
            ContractRewardFactory rewardFactory = new ContractRewardFactory();
            foreach(var re in rewardElements ) {
                var reward = rewardFactory.Create(new UniXMLElement(re));
                if(reward != null ) {
                    rewards.Add(reward);
                }
            }
        }
    }
}
