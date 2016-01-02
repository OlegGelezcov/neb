using System.Collections.Concurrent;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Resources.Inaps {
    public class InapCollection {
        private readonly ConcurrentDictionary<string, InapItem> m_Items = new ConcurrentDictionary<string, InapItem>();

        public void Load(string file) {
            m_Items.Clear();
            XDocument document = XDocument.Load(file);
            var dump = document.Element("game_inaps").Elements("game_inap").Select(element => {
                var inap = new InapItem(element);
                m_Items.TryAdd(inap.id, inap);
                return inap;
            }).ToList();
        }

        public ConcurrentDictionary<string, InapItem> inaps {
            get {
                return m_Items;
            }
        }

        public InapItem GetInap(string id) {
            InapItem resultItem;
            if(m_Items.TryGetValue(id, out resultItem)) {
                return resultItem;
            }
            return null;
        }
    }
}
