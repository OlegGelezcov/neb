using System.Collections.Generic;
using System.Linq;

namespace Nebula.Client.Res {
    public class ContractItemDataCollection {
        private Dictionary<string, ContractItemData> m_Items;

        public ContractItemDataCollection() {
            m_Items = new Dictionary<string, ContractItemData>();
        }

        public void Load(string xmlText) {
            if (m_Items == null) {
                m_Items = new Dictionary<string, ContractItemData>();
            }
            m_Items.Clear();

            UniXmlDocument document = new UniXmlDocument(xmlText);
            var dump = document.document.Element("contract_items").Elements("item").Select(itemElement => {
                ContractItemData data = new ContractItemData(new UniXMLElement(itemElement));
                m_Items.Add(data.id, data);
                return data;
            }).ToList();
        }

        public ContractItemData GetItem(string id) {
            if(m_Items.ContainsKey(id) ) {
                return m_Items[id];
            }
            return null;
        }

        public List<string> GetIds() {
            return m_Items.Keys.ToList();
        }

        public int count {
            get {
                return m_Items.Count;
            }
        }
    }
}
