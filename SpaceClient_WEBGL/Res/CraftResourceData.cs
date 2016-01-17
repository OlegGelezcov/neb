using Common;
using System;

#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class CraftResourceData {
        private string m_Id;
        private string m_Name;
        private string m_Description;
        private ObjectColor m_Color;
        private string m_Icon;
#if UP
        public CraftResourceData(UPXElement element) {
            m_Id = element.GetString("id");
            m_Name = element.GetString("name");
            m_Description = element.GetString("description");
            m_Color = (ObjectColor)Enum.Parse(typeof(ObjectColor), element.GetString("color"));
            m_Icon = element.GetString("icon");
        }
#else
        public CraftResourceData(XElement element) {
            m_Id =      element.GetString("id");
            m_Name =    element.GetString("name");
            m_Description = element.GetString("description");
            m_Color = (ObjectColor)Enum.Parse(typeof(ObjectColor), element.GetString("color"));
            m_Icon = element.GetString("icon");
        }
#endif
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
        public ObjectColor color {
            get {
                return m_Color;
            }
        }

        public string icon {
            get {
                return m_Icon;
            }
        }
    }
}
