﻿using System.Collections.Generic;
#if UP
using System.Xml;
#endif

namespace Nebula.Client.UP {
#if UP
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

        public string GetString(string name) {
            return GetAttributeString(name);
        }

        public int GetInt(string name) {
            return GetAttributeInt(name);
        }


        public float GetFloat(string name) {
            return GetAttributeFloat(name);
        }

        public bool GetBool(string name) {
            return GetAttributeBool(name);
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

        public List<UPXElement> Elements() {
            List<UPXElement> result = new List<UPXElement>();
            foreach(XmlNode element in mElement.ChildNodes) {
                result.Add(new UPXElement(element));
            }
            return result;
        }

        public string name {
            get {
                return mElement.Name;
            }
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
#endif
}
