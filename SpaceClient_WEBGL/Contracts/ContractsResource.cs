using System.Collections.Generic;
using System.Linq;

namespace Nebula.Client.Contracts {
    public class ContractsResource {
        public Dictionary<string, ContractData> contracts { get; private set; }

        public ContractsResource() {
            contracts = new Dictionary<string, ContractData>();
        }

        public void Load(string text) {
            contracts.Clear();
            UniXmlDocument document = new UniXmlDocument(text);
            var dump = document.document.Element("contracts").Elements("contract").Select(ce => {
                var contract = new ContractData(new UniXMLElement(ce));
                if(false == contracts.ContainsKey(contract.id)) {
                    contracts.Add(contract.id, contract);
                }
                return contract;
            }).ToList();

        }

        public ContractData GetContractData(string id) {
            if(contracts.ContainsKey(id)) {
                return contracts[id];
            }
            return null;
        }
    }
}
