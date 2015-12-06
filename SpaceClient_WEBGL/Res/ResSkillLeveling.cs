using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Client.Res {
    public class ResSkillLeveling {
        public Dictionary<Workshop, WorkshopSkillLeveling> workshopSkills { get; private set; }

        public void Clear() {
            workshopSkills = new Dictionary<Workshop, WorkshopSkillLeveling>();
        }

        public void Add(string xml) {
            XDocument document = XDocument.Parse(xml);
            string workshopsRaw = document.Element("skill_dropping").GetString("workshops");
            List<Workshop> workshops = workshopsRaw.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => {
                return (Workshop)Enum.Parse(typeof(Workshop), s);
            }).ToList();

            foreach(var w in workshops ) {
                if(!workshopSkills.ContainsKey(w)) {
                    workshopSkills.Add(w, new WorkshopSkillLeveling(w, document.Element("skill_dropping")));
                }
            }
        }

        public WorkshopSkillLeveling GetSkills(Workshop workshop) {
            if(workshopSkills.ContainsKey(workshop)) {
                return workshopSkills[workshop];
            }
            return null;
        }
    }
}
