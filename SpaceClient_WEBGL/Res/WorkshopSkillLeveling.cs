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


namespace Nebula.Client.Res {
    public class WorkshopSkillLeveling {
        public Workshop workshop { get; private set; }
        public Dictionary<ShipModelSlotType, ModuleSkillLeveling> moduleSkills { get; private set; }
#if UP
        public WorkshopSkillLeveling(Workshop w, UPXElement element) {
            workshop = w;
            moduleSkills = element.Elements("slot").Select(e => {
                return new ModuleSkillLeveling(e);
            }).ToDictionary(m => m.slotType, m => m);
        }
#else
        public WorkshopSkillLeveling(Workshop w, XElement element ) {
            workshop = w;
            moduleSkills = element.Elements("slot").Select(e => {
                return new ModuleSkillLeveling(e);
            }).ToDictionary(m => m.slotType, m => m);
        }
#endif
        public ModuleSkillLeveling GetModuleSkills(ShipModelSlotType slotType) {
            if(moduleSkills.ContainsKey(slotType)) {
                return moduleSkills[slotType];
            }
            return null;
        }
    }
}
