using Common;
using System;
using System.Collections;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Resources.Inaps {

    public class InapItem {
        private string m_Id;
        private string m_Name;
        private string m_Desc;
        private int m_Price;
        private InapObjectType m_Type;
        private bool m_Consumable;
        private Hashtable m_Data;
        private int m_Tag;

        public InapItem(XElement element) {
            m_Id = element.GetString("id");
            m_Name = element.GetString("name");
            m_Desc = element.GetString("description");
            m_Price = element.GetInt("price");
            m_Type = (InapObjectType)Enum.Parse(typeof(InapObjectType), element.GetString("type"));
            m_Consumable = element.GetBool("consumable");
            m_Tag = element.GetInt("tag");

            m_Data = new Hashtable();
            var inputsElement = element.Element("inputs");
            if(inputsElement != null ) {
                inputsElement.Elements("input").Select(ie => {
                    string key = ie.GetString("key");
                    object val = CommonUtils.ParseValue(ie.GetString("value"), ie.GetString("type"));
                    m_Data.Add(key, val);
                    return key;
                }).ToList();
            }
        }

        public string id {
            get {
                return m_Id;
            }
        }

        public string name {
            get {
                return m_Name;
            }
        }

        public string description {
            get {
                return m_Desc;
            }
        }

        public int price {
            get {
                return m_Price;
            }
        }

        public InapObjectType type {
            get {
                return m_Type;
            }
        }

        public bool consumable {
            get {
                return m_Consumable;
            }
        }

        public Hashtable data {
            get {
                return m_Data;
            }
        }

        public int tag {
            get {
                return m_Tag;
            }
        }
    }
}
