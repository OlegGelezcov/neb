using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Client.Res {
    public class ResWorkshopStats {
        public Dictionary<Workshop, WorkshopStatData> stats { get; private set; }

        public void Load(string xml) {
            XDocument document = XDocument.Parse(xml);
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
