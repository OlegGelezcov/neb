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
    public class PetPlayerCount {
        private int m_MinLevel;
        private int m_MaxLevel;
        private int m_Count;
#if UP
        public PetPlayerCount(UPXElement element) {
            m_MinLevel = element.GetInt("min");
            m_MaxLevel = element.GetInt("max");
            m_Count = element.GetInt("count");
        }
#else
        public PetPlayerCount(XElement element) {
            m_MinLevel = element.GetInt("min");
            m_MaxLevel = element.GetInt("max");
            m_Count = element.GetInt("count");
        }
#endif
        public int minLevel {
            get {
                return m_MinLevel;
            }
        }

        public int maxLevel {
            get {
                return m_MaxLevel;
            }
        }

        public int count {
            get {
                return m_Count;
            }
        }
    }
}
