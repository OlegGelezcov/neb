
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
using Nebula.Client.UP;
#endif
namespace Nebula.Client {
    public class UniXMLElement {

#if UP
        public UPXElement element;

        public UniXMLElement(UPXElement e) {
            element = e;
        }
#else
        public XElement element;

        public UniXMLElement(XElement e) {
            element = e;
        }
#endif

        public UniXMLElement() { }




    }
}
