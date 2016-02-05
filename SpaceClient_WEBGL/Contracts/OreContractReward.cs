using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
using Nebula.Client.UP;
#endif
namespace Nebula.Client.Contracts {
    public class OreContractReward : ContractReward {

        public int minCount { get; private set; }
        public int maxCount { get; private set; }

        public OreContractReward(UPXElement element)
            : base(element) {
            minCount = element.GetInt("min_count");
            maxCount = element.GetInt("max_count");
        }

        public OreContractReward(XElement element)
            : base(element) {
            minCount = element.GetInt("min_count");
            maxCount = element.GetInt("max_count");
        }


    }
}
