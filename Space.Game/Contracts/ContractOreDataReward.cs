using Common;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class ContractOreDataReward : ContractDataReward {
        public int minCount { get; private set; }
        public int maxCount { get; private set; }

        public ContractOreDataReward(XElement element)
            : base(element) {
            minCount = element.GetInt("min_count");
            maxCount = element.GetInt("max_count");
        }

    }
}
