using System.Xml.Linq;

namespace Nebula.Contracts {
    public class ContractResource : IContractResource {

        public ContractDataCollection contracts {
            get;
            private set;
        }

        public void Load(string file ) {
            XDocument document = XDocument.Load(file);
            contracts = new ContractDataCollection(document.Element("contracts"));
        }
    }
}
