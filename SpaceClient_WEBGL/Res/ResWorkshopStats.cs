using Common;
using System.Collections.Generic;
using System.Linq;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class ResWorkshopStats {
        public Dictionary<Workshop, WorkshopStatData> stats { get; private set; }

        public void Load(string xml) {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
            stats = document.Element("stats").Elements("workshop").Select(e => {
                WorkshopStatData stat = new WorkshopStatData(e);
                return stat;
            }).ToDictionary(s => s.workshop, s => s);
        }


        public WorkshopStatData GetStat(Workshop workshop) {
            if(stats.ContainsKey(workshop)) {
                return stats[workshop];
            }
            return null;
        }
    }
}
