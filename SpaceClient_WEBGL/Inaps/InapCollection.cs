using System.Collections.Generic;
using System.Linq;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Inaps {
    public class InapCollection {
        private Dictionary<string, InapItem> m_Items;

        public InapCollection() {
            m_Items = new Dictionary<string, InapItem>();
        }

        public void Load(string xml) {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
            m_Items = document.Element("items").Elements("item").Select(itemElement => {
                InapItem item = new InapItem(itemElement);
                return item;
            }).ToDictionary(item => item.id, item => item);
        }

        public Dictionary<string, InapItem> items {
            get {
                return m_Items;
            }
        }

        public List<InapItem> orderedItems {
            get {
                List<InapItem> list = new List<InapItem>();
                foreach(var kvp in items) {
                    list.Add(kvp.Value);
                }
                list.Sort((it1, it2) => {
                    return it1.id.CompareTo(it2.id);
                });
                return list;
            }
        }

        public List<InapItem> GetFilteredInaps(InapType type) {
            return orderedItems.Where(item => item.type == type).ToList();
        }

        public InapItem GetItem(string id ) {
            if(m_Items.ContainsKey(id)) {
                return m_Items[id];
            }
            return null;
        }
    }
}
