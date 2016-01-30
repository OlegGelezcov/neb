using Common;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Nebula.Resources.NpcSkills {
    public class NpcColorSkillList {
        public ObjectColor color { get; private set; }
        public List<int> skills { get; private set; }

        public NpcColorSkillList(XElement element) {
            color = (ObjectColor)Enum.Parse(typeof(ObjectColor), element.GetString("name"));

            string skillString = element.GetString("skills");
            string[] skillStringTokens = skillString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            skills = new List<int>();
            foreach(var token in skillStringTokens) {
                int skillId = Convert.ToInt32(token, 16);
                skills.Add(skillId);
            }
        }
    }
}
