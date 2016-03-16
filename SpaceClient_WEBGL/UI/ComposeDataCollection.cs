using System.Collections.Generic;
using System.Linq;

namespace Nebula.Client.UI {
    public class ComposeDataCollection {
        private Dictionary<string, ComposeData> m_ComposeCollection;

        public void Load(string xml, string lang) {
            UniXmlDocument document = new UniXmlDocument(xml);
            UniXMLElement langElement = null;
            switch(lang) {
                case "ru": {
                        langElement = new UniXMLElement(document.document.Element("compose").Element("ru"));
                    }
                    break;
                default: {
                        langElement = new UniXMLElement(document.document.Element("compose").Element("en"));
                    }
                    break;
            }

            m_ComposeCollection = new Dictionary<string, ComposeData>();
            var dump = langElement.Elements("data").Select(e => {
                ComposeData composeData = new ComposeData(e);
                m_ComposeCollection.Add(composeData.name, composeData);
                return composeData;
            }).ToList();
        }

        public Dictionary<string, ComposeData> collection {
            get {
                return m_ComposeCollection;
            }
        }

        public ComposeData GetData(string name) {
            if(collection.ContainsKey(name)) {
                return collection[name];
            }
            return null;
        }
    }
}
