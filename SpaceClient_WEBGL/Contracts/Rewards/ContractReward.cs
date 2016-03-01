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
        public string name { get; private set; }

#if UP
        public ContractReward(UPXElement element) : this(new UniXMLElement(element)) {
        }
#else

        public ContractReward(XElement element) : this(new UniXMLElement(element)) {
        }
#endif

        public ContractReward(UniXMLElement element) {
            rewardType = (ContractRewardType)System.Enum.Parse(typeof(ContractRewardType), element.element.GetString("type"));
            if(element.HasAttribute("name")) {
                name = element.GetString("name");
            } else {
                name = string.Empty;
            }
        }
    }
}
