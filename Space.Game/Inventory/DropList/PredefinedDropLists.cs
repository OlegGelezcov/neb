using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Inventory.DropList {
    public class PredefinedDropLists {
        private ConcurrentDictionary<string, List<DropItem>> m_DropLists;

        public PredefinedDropLists() {
            m_DropLists = new ConcurrentDictionary<string, List<DropItem>>();
        }

        public void Load(string file) {
            m_DropLists.Clear();

            DropListFactory factory = new DropListFactory();
            XDocument document = XDocument.Load(file);
            var dump = document.Element("drop_lists").Elements("drop_list").Select(dlElement => {
                string name = dlElement.GetString("name");
                var list = factory.Create(dlElement);
                m_DropLists.TryAdd(name, list);
                return name;
            }).ToList();
        }

        public List<DropItem> GetDropList(string name) {
            List<DropItem> result = null;
            if(m_DropLists.TryGetValue(name, out result)) {
                return result;
            }
            return new List<DropItem>();
        }
    }
}
