using System.Xml.Linq;

namespace Nebula.Contracts {
    public class UpgradePetContractData : ContractData {
        public UpgradePetContractData(XElement element)
            : base(element) { }
    }
}
