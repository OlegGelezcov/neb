
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Pets {
    public class PetPassiveBonus {
        private int m_Id;
        private string m_Name;
        private string m_Desc;
        private string m_Icon;

        public PetPassiveBonus(object elementObj) {
#if UP
            var element = elementObj as UPXElement;
#else
            var element = elementObj as XElement;
#endif
            m_Id = element.GetInt("id");
            m_Name = element.GetString("name");
            m_Desc = element.GetString("desc");
            m_Icon = element.GetString("icon");

        }

        public int id {
            get {
                return m_Id;
            }
        }

        public string name {
            get {
                return m_Name;
            }
        }

        public string desc {
            get {
                return m_Desc;
            }
        }

        public string icon {
            get {
                return m_Icon;
            }
        }
    }
}
