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
    public class PetUpgradeRequirement {
        private PetColor m_Color;
        private List<IDCountPair> m_Entries;

#if UP
        public PetUpgradeRequirement(UPXElement element) {
            m_Color = (PetColor)Enum.Parse(typeof(PetColor), element.GetString("color"));
            m_Entries = element.Elements("element").Select(e => {
                IDCountPair pair = new IDCountPair {
                    ID = e.GetString("id"),
                    count = e.GetInt("count")
                };
                return pair;
            }).ToList();
        }
#else
        public PetUpgradeRequirement(XElement element) {
            m_Color = (PetColor)Enum.Parse(typeof(PetColor), element.GetString("color"));
            m_Entries = element.Elements("element").Select(e => {
                IDCountPair pair = new IDCountPair {
                    ID = e.GetString("id"),
                    count = e.GetInt("count")
                };
                return pair;
            }).ToList();
        }
#endif

        public PetColor color {
            get {
                return m_Color;
            }
        }

        public List<IDCountPair> entries {
            get {
                return m_Entries;
            }
        }
    }
}
