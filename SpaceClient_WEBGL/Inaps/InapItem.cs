using Common;

#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Inaps {
    public class InapItem {

        private string m_Id;
        private string m_Name;
        private string m_Description;
        private int m_Count;
        private string m_Price;
        private string m_Icon;
        private InapType m_Type;

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
                return m_Description;
            }
        }

        public int count {
            get {
                return m_Count;
            }
        }

        public string price {
            get {
                return m_Price;
            }
        }

        public string icon {
            get {
                return m_Icon;
            }
        }

        public InapType type {
            get {
                return m_Type;
            }
        }

#if UP
        public InapItem(UPXElement element) {
            m_Id = element.GetString("id");
            m_Name = element.GetString("name");
            m_Description = element.GetString("description");
            m_Count = element.GetInt("count");
            m_Price = element.GetString("price");
            m_Icon = element.GetString("icon");
            m_Type = (InapType)System.Enum.Parse(typeof(InapType), element.GetString("type"));
        }
#else
        public InapItem(XElement element) {
            m_Id = element.GetString("id");
            m_Name = element.GetString("name");
            m_Description = element.GetString("description");
            m_Count = element.GetInt("count");
            m_Price = element.GetString("price");
            m_Icon = element.GetString("icon");
            m_Type = (InapType)System.Enum.Parse(typeof(InapType), element.GetString("type"));
        }
#endif

    }
}
