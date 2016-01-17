using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Nebula.Client.Res {
    public class PetData {
        private string m_Id;
        private Race m_Race;
        private string m_Name;
        private string m_Icon;

#if UP
        public PetData(UPXElement element) {
            m_Id = element.GetString("id");
            m_Race = (Race)Enum.Parse(typeof(Race), element.GetString("race"));
            m_Name = element.GetString("name");
            m_Icon = element.GetString("icon");
        }
#else
        public PetData(XElement element) {
            m_Id = element.GetString("id");
            m_Race = (Race)Enum.Parse(typeof(Race), element.GetString("race"));
            m_Name = element.GetString("name");
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

        public Race race {
            get {
                return m_Race;
            }
        }

        public string icon {
            get {
                return m_Icon;
            }
        }

    }
}
