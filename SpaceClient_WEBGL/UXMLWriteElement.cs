using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client {
    public class UXMLWriteElement {

        public string name { get; private set; }
        public List<UXMLWriteElement> childrens { get; private set; }
        public List<UXMLAttribute> attributes { get; private set; }

        public UXMLWriteElement(string name) {
            this.name = name;
            this.childrens = new List<UXMLWriteElement>();
            this.attributes = new List<UXMLAttribute>();
        }

        public void AddAttribute(string name, string value) {
            attributes.Add(new UXMLAttribute(name, value));
        }

        public void AddAttribute(string name, int value) {
            attributes.Add(new UXMLAttribute(name, value));
        }

        public void AddAttribute(string name, float value) {
            attributes.Add(new UXMLAttribute(name, value));
        }

        public void AddAttribute(string name, bool value) {
            attributes.Add(new UXMLAttribute(name, value));
        }

        public void Add(UXMLWriteElement element) {
            childrens.Add(element);
        }

        public override string ToString() {
#if UNITY_IOS
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true });
            WriteToXmlWriter(writer);
            writer.Close();
            return sb.ToString();
#else
            return WriteToXElement(null).ToString();
#endif
        }

#if UNITY_IOS
        private void WriteToXmlWriter(XmlWriter writer) {
            writer.WriteStartElement(name);
            foreach (var attr in attributes) {
                writer.WriteAttributeString(attr.name, attr.value);
            }
            foreach (var child in childrens) {
                child.WriteToXmlWriter(writer);
            }
            writer.WriteEndElement();
        }
#else
        private XElement WriteToXElement(XElement parent) {
            XElement current = new XElement(name);
            foreach (var attr in attributes) {
                current.SetAttributeValue(attr.name, attr.value);
            }

            foreach (var ch in childrens) {
                ch.WriteToXElement(current);
            }

            if (parent != null) {
                parent.Add(current);
            }

            if (parent != null) {
                return parent;
            } else {
                return current;
            }
        }
#endif

    }
}
