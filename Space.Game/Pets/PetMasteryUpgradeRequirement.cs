using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Pets {
    public class PetMasteryUpgradeRequirement {
        private List<IDCountPair> m_Entries;
        private int m_Mastery;

        public PetMasteryUpgradeRequirement(XElement element) {
            m_Mastery = element.GetInt("mastery");
            m_Entries = element.Elements("element").Select(e => {
                return new IDCountPair(e);
            }).ToList();
        }

        public List<IDCountPair> entries {
            get {
                return m_Entries;
            }
        }

        public int mastery {
            get {
                return m_Mastery;
            }
        }
    }
}
