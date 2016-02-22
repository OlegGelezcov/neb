using Common;
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
    }
}
