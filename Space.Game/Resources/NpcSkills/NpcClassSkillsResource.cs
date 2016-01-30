using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources.NpcSkills {
    public class NpcClassSkillsResource {
        public ConcurrentDictionary<NpcClass, NpcClassSkillMap> classSkills { get; private set; }

        public void Load(string file) {
            classSkills = new ConcurrentDictionary<NpcClass, NpcClassSkillMap>();
            XDocument document = XDocument.Load(file);
            var dump = document.Element("skills").Elements("class").Select(classElement => {
                NpcClassSkillMap classMap = new NpcClassSkillMap(classElement);
                classSkills.TryAdd(classMap.npcClass, classMap);
                return classMap;
            }).ToList();
        }


        public NpcClassSkillMap GetClassSkillMap(NpcClass cl) {
            NpcClassSkillMap result;
            if(classSkills.TryGetValue(cl, out result)) {
                return result;
            }
            return null;
        }
    }
}
