using System.Collections.Generic;
using System.Xml;


namespace Nebula.Client.UP {
    public class UPXElement {
        private XmlNode mElement;

        public UPXElement(XmlNode element) {
            mElement = element;
        }

        public XmlAttribute Attribute(string name) {
            foreach(XmlAttribute attr in mElement.Attributes) {
                if(attr.Name == name ) {
                    return attr;
                }
            }
            return null;
        }

        public bool HasAtt(string name) {
            foreach(XmlAttribute attr in mElement.Attributes) {
                if(attr.Name == name ) {
                    return true;
                }
            }
            return false;
        }

        public string GetAttributeString(string name) {
            foreach(XmlAttribute attr in mElement.Attributes) {
                if(attr.Name == name ) {
                    return attr.Value;
                }
            }
            return string.Empty;
        }

        public int GetAttributeInt(string name) {
            
            foreach(XmlAttribute attr in mElement.Attributes) {
                if(attr.Name == name) {
                    int iResult = 0;
                    if (int.TryParse(attr.Value, out iResult)) {
                        return iResult;
                    }
                }
            }
            return 0;
        }

        public float GetAttributeFloat(string name) {
            foreach(XmlAttribute attr in mElement.Attributes) {
                if(attr.Name == name) {
                    float fResult = 0f;
                    if(float.TryParse(attr.Value, System.Globalization.NumberStyles.Any,  System.Globalization.CultureInfo.InvariantCulture, out fResult)) {
                        return fResult;
                    }
                }
            }
            return 0f;
        }

        public bool GetAttributeBool(string name) {
            foreach(XmlAttribute attr in mElement.Attributes) {
                if(attr.Name == name) {
                    bool bResult = false;
                    if(bool.TryParse(attr.Value, out bResult)) {
                        return bResult;
                    }
                }
            }
            return false;
        }

        public string value {
            get {
                return mElement.InnerText;
            }
        }

        public string Value {
            get {
                return this.value;
            }
        }

        public List<UPXElement> Elements(string name) {
            List<UPXElement> mLstResult = new List<UPXElement>();
            foreach(XmlNode element in mElement.ChildNodes) {
                if(element.Name == name ) {
                    mLstResult.Add(new UPXElement(element));
                }
            }
            return mLstResult;
        }

        public UPXElement Element(string name) {
            UPXElement mResult = null;
            foreach(XmlNode element in mElement.ChildNodes) {
                if(element.Name == name ) {
                    mResult = new UPXElement(element);
                    break;
                }
            }
            return mResult;
        }
    }
}
