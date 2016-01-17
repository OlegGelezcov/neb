
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class CraftResourceCollection {

        private Dictionary<string, CraftResourceData> m_CraftResources;

        public void Load(string xml) {
            m_CraftResources = new Dictionary<string, CraftResourceData>();
#if UP
            UPXDocument document = new UPXDocument(xml); 
#else
            XDocument document = XDocument.Parse(xml);
#endif
            var dump = document.Element("resources").Elements("res").Select(element => {
                CraftResourceData data = new CraftResourceData(element);
                m_CraftResources.Add(data.id, data);
                return data;
            }).ToList();
        }

        public CraftResourceData GetData(string id) {
            if(m_CraftResources.ContainsKey(id)) {
                return m_CraftResources[id];
            }
            return null;
        }
    }
}
