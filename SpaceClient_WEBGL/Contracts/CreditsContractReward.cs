using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
using Nebula.Client.UP;
#endif
namespace Nebula.Client.Contracts {
    public class CreditsContractReward : ContractReward {
        public int count { get; private set; }

        public CreditsContractReward(UPXElement element) : base(element) {
            count = element.GetInt("count");
        }

        public CreditsContractReward(XElement element) : base(element) {
            count = element.GetInt("count");
        }


    }
}
