using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Pets {
    public class PetUpgradeRequirement {
        private List<IDCountPair> m_Entries;
        private PetColor m_Color;

        public PetUpgradeRequirement(XElement element) {
            m_Color = (PetColor)Enum.Parse(typeof(PetColor), element.GetString("color"));
            m_Entries = element.Elements("element").Select(e => {
                return new IDCountPair(e);
            }).ToList();
        }

        public List<IDCountPair> entries {
            get {
                return m_Entries;
            }
        }

        public PetColor color {
            get {
                return m_Color;
            }
        }
    }
}
