using Common;
using System;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class CraftResourceObjectData {
        private string m_Id;
        private ObjectColor m_Color;

        public CraftResourceObjectData(XElement element) {
            m_Id = element.GetString("id");
            m_Color = (ObjectColor)Enum.Parse(typeof(ObjectColor), element.GetString("color"));
        }

        public string id {
            get {
                return m_Id;
            }
        }

        public ObjectColor color {
            get {
                return m_Color;
            }
        }
    }
}
