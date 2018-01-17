using Nebula.Client.UP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UP
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client {
    public class UniXmlDocument {

#if UP
        public UPXDocument document;
        public UniXmlDocument(UPXDocument doc) {
            document = doc;
        }
#else
        public XDocument document;
        public UniXmlDocument(XDocument doc) {
            document = doc;
        }
#endif

        public UniXmlDocument(string text) {
#if UP
            document = new UPXDocument(text);
#else
            document = XDocument.Parse(text);
#endif
        }

        public UniXMLElement Element(string name) {
            var rawElement = document.Element(name);
            if(rawElement != null ) {
                return new UniXMLElement(rawElement);
            }
            return null;
        }



    }
}
