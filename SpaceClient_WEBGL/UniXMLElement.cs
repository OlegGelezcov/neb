using Common;
using System.Collections.Generic;
using System.Linq;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
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


        public string GetString(string key) {
            return element.GetString(key);
        }

        public int GetInt(string key) {
            return element.GetInt(key);
        }

        public bool GetBool(string key) {
            return element.GetBool(key);
        }

        public float GetFloat(string key) {
            return element.GetFloat(key);
        }

        public string innerValue {
            get {
#if UP
                return element.value;
#else
                return element.Value;
#endif
            }
        }

        public bool HasAttribute(string name) {
#if UP
            if(element.HasAtt(name)) {
                return true;
            }
#else
            if(element.HasAttribute(name)) {
                return true;
            }
#endif
            return false;
        }

        public List<UniXMLElement> Elements() {
            return element.Elements().Select(e => {
                return new UniXMLElement(e);
            }).ToList();
        }

        public UniXMLElement Element(string name) {
            return new UniXMLElement(element.Element(name));
        }

        public string name {
            get {
#if UP
                return element.name;  
#else
                return element.Name.ToString();
#endif
            }
        }

        public List<UniXMLElement> Elements(string name) {
            return element.Elements(name).Select(e => {
                return new UniXMLElement(e);
            }).ToList();
        }
    }
}
