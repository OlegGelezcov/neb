namespace Nebula.Resources {
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

#if UNITY_IPHONE
using System.Xml;
using System.Xml.Linq;
using System.Linq;
#else
    using System.Xml;
    using System.Linq;
    using System.Xml.Linq;
#endif



    public class sXDocument {
#if UNITY_IPHONE
        private XmlDocument xmlDocument;
#else
        private XDocument xDocument;
#endif

        public sXDocument() {
#if UNITY_IPHONE
            xmlDocument = new XmlDocument();
#else
            xDocument = new XDocument();
#endif
        }

        public void Parse(string resourcePath) {

            TextAsset textAsset = UnityEngine.Resources.Load<TextAsset>(resourcePath) as TextAsset;
#if UNITY_IPHONE
            xmlDocument.LoadXml(textAsset.text);
#else
            xDocument = XDocument.Parse(textAsset.text);
#endif
        }

        public void ParseXml(string xml) {
#if UNITY_IPHONE
            xmlDocument.LoadXml(xml);
#else
            xDocument = XDocument.Parse(xml);
#endif        
        }

        public List<sXElement> GetElements(string name) {
            List<sXElement> result = new List<sXElement>();
#if UNITY_IPHONE
            foreach (XmlNode n in xmlDocument.ChildNodes) {
                if (n.Name == name) {
                    result.Add(new sXElement(n));
                }
            }
#else
            result = xDocument.Elements(name).Select((e) => new sXElement(e)).ToList();
#endif
            return result;
        }

        public sXElement GetElement(string name) {
            sXElement result = null;
#if UNITY_IPHONE
            foreach (XmlNode n in xmlDocument.ChildNodes) {
                if (n.Name == name) {
                    result = new sXElement(n);
                    break;
                }
            }
#else
            result = new sXElement(xDocument.Element(name));
#endif
            return result;
        }

        public List<sXElement> GetElements() {
            List<sXElement> result = new List<sXElement>();

#if UNITY_IPHONE
            foreach (XmlNode n in xmlDocument.ChildNodes)
            {
                result.Add(new sXElement(n));
            }
#else
            result = xDocument.Elements().Select(e => new sXElement(e)).ToList();
#endif
            return result;
        }

    }

    public class sXAttribute {
#if UNITY_IPHONE
        private XmlAttribute xmlAttribute;
#else
        private XAttribute xAttribute;
#endif

        public string Name {
            get {
#if UNITY_IPHONE
                return xmlAttribute.Name;
#else
                return xAttribute.Name.ToString();
#endif
            }
        }

        public string Value {
            get {
#if UNITY_IPHONE
                return xmlAttribute.Value;
#else
                return xAttribute.Value;
#endif
            }
        }

#if UNITY_IPHONE
        public sXAttribute(XmlAttribute attr)
        {
            xmlAttribute = attr;
        }
#else
        public sXAttribute(XAttribute attr) {
            xAttribute = attr;
        }
#endif
    }

    public class sXElement {
#if UNITY_IPHONE
        private XmlNode xmlNode;
#else
        private XElement xElement;
#endif
#if UNITY_IPHONE
        public sXElement(XmlNode node) {
            xmlNode = node;
        }
#else
        public sXElement(XElement element) {
            xElement = element;
        }
#endif

        public string Name {
            get {
#if UNITY_IPHONE
                return xmlNode.Name;
#else
                return xElement.Name.ToString();
#endif

            }
        }
        public List<sXElement> GetElements(string name) {
            List<sXElement> result = new List<sXElement>();
#if UNITY_IPHONE
            foreach (XmlNode n in xmlNode.ChildNodes) {
                if (n.Name == name) {
                    result.Add(new sXElement(n));
                }
            }
#else
            result = xElement.Elements(name).Select(e => new sXElement(e)).ToList();
#endif
            return result;
        }

        public List<sXElement> GetElements() {
            List<sXElement> result = new List<sXElement>();

#if UNITY_IPHONE
            foreach (XmlNode n in xmlNode.ChildNodes)
            {
                result.Add(new sXElement(n));
            }
#else
            result = xElement.Elements().Select(e => new sXElement(e)).ToList();
#endif
            return result;
        }



        public sXElement GetElement(string name) {
            sXElement result = null;
#if UNITY_IPHONE
            foreach (XmlNode n in xmlNode.ChildNodes) {
                if (n.Name == name) {
                    result = new sXElement(n);
                    break;
                }
            }
#else
            if (xElement != null && xElement.Element(name) != null)
                result = new sXElement(xElement.Element(name));
            else
                result = null;
#endif
            return result;
        }

        public int GetInt(string name) {
#if UNITY_IPHONE
            return int.Parse(xmlNode.Attributes[name].Value);
#else
            return int.Parse(xElement.Attribute(name).Value);
#endif
        }

        public float GetFloat(string name) {
#if UNITY_IPHONE
            return float.Parse(xmlNode.Attributes[name].Value, System.Globalization.CultureInfo.InvariantCulture);
#else
            return float.Parse(xElement.Attribute(name).Value, System.Globalization.CultureInfo.InvariantCulture);
#endif
        }

        public T GetEnum<T>(string name) {
#if UNITY_IPHONE
            return (T)System.Enum.Parse(typeof(T), xmlNode.Attributes[name].Value);
#else
            return (T)System.Enum.Parse(typeof(T), xElement.Attribute(name).Value);
#endif
        }

        public bool GetBool(string name) {
#if UNITY_IPHONE
            return bool.Parse(xmlNode.Attributes[name].Value);
#else
            return bool.Parse(xElement.Attribute(name).Value);
#endif
        }

        public string GetString(string name) {
#if UNITY_IPHONE
            return xmlNode.Attributes[name].Value;
#else
            return xElement.Attribute(name).Value;
#endif     
        }

        public string GetValue() {
#if UNITY_IPHONE
            return xmlNode.InnerText;
#else
            return xElement.Value;
#endif
        }

        public List<sXAttribute> Attributes {
            get {
#if UNITY_IPHONE
                List<sXAttribute> attributes = new List<sXAttribute>();
                foreach (XmlAttribute attr in xmlNode.Attributes)
                {
                    attributes.Add(new sXAttribute(attr));
                }
                return attributes;
#else
                return xElement.Attributes().Select((attr) => {
                    return new sXAttribute(attr);
                }).ToList();
#endif
            }
        }

        public bool HasAttribute(string name) {
            foreach (var attr in this.Attributes) {
                if (attr.Name == name)
                    return true;
            }
            return false;
        }


        public sXAttribute Attribute(string name) {
            return Attributes.FirstOrDefault((attr) => attr.Name == name);
        }
    }
}