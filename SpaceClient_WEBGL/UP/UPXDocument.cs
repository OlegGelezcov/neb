using System.Collections.Generic;
#if UP
using System.Xml;
#endif

namespace Nebula.Client.UP {
#if UP
    public class UPXDocument {
        private XmlDocument mDocument;

        public UPXDocument() {
            mDocument = new XmlDocument();
        }

        public void LoadFromFile(string file ) {
            mDocument.Load(file);
        }

        public UPXDocument(string xml) {
            mDocument = new XmlDocument();
            mDocument.LoadXml(xml);
        }

        public List<UPXElement> Elements(string name) {
            List<UPXElement> mLstResult = new List<UPXElement>();
            foreach (XmlNode element in mDocument.ChildNodes) {
                if (element.Name == name) {
                    mLstResult.Add(new UPXElement(element));
                }
            }
            return mLstResult;
        }

        public UPXElement Element(string name) {
            UPXElement mResult = null;
            foreach (XmlNode element in mDocument.ChildNodes) {
                if (element.Name == name) {
                    mResult = new UPXElement(element);
                    break;
                }
            }
            return mResult;
        }
    }
#endif
}
