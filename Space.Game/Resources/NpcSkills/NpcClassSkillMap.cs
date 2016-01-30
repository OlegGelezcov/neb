using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources.NpcSkills {
    public class NpcClassSkillMap {
        public NpcClass npcClass { get; private set; }
        public ConcurrentDictionary<ObjectColor, NpcColorSkillList> colorSkillMap { get; private set; }

        public NpcClassSkillMap(XElement element) {
            npcClass = (NpcClass)Enum.Parse(typeof(NpcClass), element.GetString("name"));
            colorSkillMap = new ConcurrentDictionary<ObjectColor, NpcColorSkillList>();
            var dump = element.Elements("color").Select(colorElement => {
                NpcColorSkillList colorList = new NpcColorSkillList(colorElement);
                colorSkillMap.TryAdd(colorList.color, colorList);
                return colorList;
            }).ToList();
        }

        public NpcColorSkillList GetColorSkillList(ObjectColor color) {
            NpcColorSkillList result = null;
            if(colorSkillMap.TryGetValue(color, out result)) {
                return result;
            }
            return null;
        }
    }
}
