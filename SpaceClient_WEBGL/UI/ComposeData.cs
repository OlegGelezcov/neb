using System.Collections.Generic;
using System.Linq;

namespace Nebula.Client.UI {
    public class ComposeData {
        private string m_Name;
        private List<ComposeElementData> m_Elements;

        public ComposeData(UniXMLElement element) {
            m_Name = element.GetString("name");
            m_Elements = new List<ComposeElementData>();

            var dump = element.Elements().Select(e => {
                switch (e.name) {
                    case "text": {
                            m_Elements.Add(new TextComposeData(e));
                        }
                        break;
                    case "image": {
                            m_Elements.Add(new ImageComposeData(e));
                        }
                        break;
                }
                return e;
            }).ToList();
        }

        public string name {
            get {
                return m_Name;
            }
        }

        public List<ComposeElementData> elements {
            get {
                return m_Elements;
            }
        }
    }
}
