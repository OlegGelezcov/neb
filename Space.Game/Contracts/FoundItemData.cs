using Common;
using Space.Game;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class FoundItemData  : ElementBaseData {

        private List<string> m_NpcZones;

        public FoundItemData(XElement element)
            : base(element) {
            m_NpcZones = element.GetString("npc_zones").Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        public FoundItemData() {
            m_NpcZones = new List<string>();
        }

        public string RandomNpcZone() {
            if(m_NpcZones.Count > 0) {
                return m_NpcZones.AnyElement();
            }
            return string.Empty;
        }

        public override void Load(XElement element) {
            base.Load(element);
            m_NpcZones = element.GetString("npc_zones").Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
