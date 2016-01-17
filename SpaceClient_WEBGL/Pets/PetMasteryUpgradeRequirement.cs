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

    public class PetMasteryUpgradeRequirement {
        private int m_Mastery;
        private List<IDCountPair> m_Entries;

#if UP
        public PetMasteryUpgradeRequirement(UPXElement element) {
            m_Mastery = element.GetInt("mastery");
            m_Entries = element.Elements("element").Select(e => {
                IDCountPair pair = new IDCountPair {
                    ID = e.GetString("id"),
                    count = e.GetInt("count")
                };
                return pair;
            }).ToList();
        }
#else
        public PetMasteryUpgradeRequirement(XElement element) {
            m_Mastery = element.GetInt("mastery");
            m_Entries = element.Elements("element").Select(e => {
                IDCountPair pair = new IDCountPair {
                    ID = e.GetString("id"),
                    count = e.GetInt("count")
                };
                return pair;
            }).ToList();
        }
#endif

        public int mastery {
            get {
                return m_Mastery;
            }
        }

        public List<IDCountPair> entries {
            get {
                return m_Entries;
            }
        }
    }
}
