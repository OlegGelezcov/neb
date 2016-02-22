using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Contracts.Inventory {

    /// <summary>
    /// Collection of contract items
    /// </summary>
    public class ContractItemDataCollection {
        private ConcurrentDictionary<string, ContractItemData> m_Items;

        public ContractItemDataCollection() {
            m_Items = new ConcurrentDictionary<string, ContractItemData>();
        }

        /// <summary>
        /// Loading collection from resource file
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path) {
            XDocument document = XDocument.Load(path);
            if (m_Items == null) {
                m_Items = new ConcurrentDictionary<string, ContractItemData>();
            }
            m_Items.Clear();

            var dump = document.Element("contract_items").Elements("item").Select(itemElement => {
                ContractItemData data = new ContractItemData(itemElement);
                m_Items.TryAdd(data.id, data);
                return data;
            }).ToList();
        }

        public ContractItemData GetItem(string id) {
            ContractItemData data;
            if(m_Items.TryGetValue(id, out data)) {
                return data;
            }
            return null;
        }

        public List<string> ids {
            get {
                return m_Items.Keys.ToList();
            }
        }
    }
}
