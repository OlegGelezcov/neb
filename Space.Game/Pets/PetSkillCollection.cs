using System.Collections.Concurrent;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Pets {
    public class PetSkillCollection {
        private ConcurrentDictionary<int, PetSkillInfo> m_Skills;

        public PetSkillCollection() {
            m_Skills = new ConcurrentDictionary<int, PetSkillInfo>();
        }

        public void Load(string file) {
            m_Skills.Clear();

            XDocument document = XDocument.Load(file);
            var dump = document.Element("skills").Elements("skill").Select(element => {
                PetSkillInfo skill = new PetSkillInfo(element);
                m_Skills.TryAdd(skill.id, skill);
                return skill.id;
            }).ToList();
        }

        public PetSkillInfo GetSkill(int id) {
            if(m_Skills.ContainsKey(id)) {
                PetSkillInfo result = null;
                if(m_Skills.TryGetValue(id, out result)) {
                    return result;
                }
            }
            return null;
        }
    }
}
