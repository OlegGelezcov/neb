using Space.Game;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public List<PetActiveSkill> GetRandomSkills(int count) {
            List<int> allSklls = new List<int>(m_Skills.Keys);
            allSklls = allSklls.Shuffle();
            var resultIds =  allSklls.Take(count).ToList();
            List<PetActiveSkill> resultSkills = new List<PetActiveSkill>();
            foreach(var id in resultIds) {
                resultSkills.Add(new PetActiveSkill { id = id, activated = true });
            }
            return resultSkills;
        }

        public List<PetActiveSkill> GetRandomSkills(int count, List<PetActiveSkill> except) {
            List<int> allSklls = new List<int>(m_Skills.Keys);
            var filtered = allSklls.Where(s => (false == ListContains(except, s))).ToList().Shuffle();
            var ids = filtered.Take(count).ToList();
            List<PetActiveSkill> result = new List<PetActiveSkill>();
            foreach(var id in ids) {
                result.Add(new PetActiveSkill { id = id, activated = true });
            }
            return result;
        }

        private bool ListContains(List<PetActiveSkill> source, int s) {
            foreach(var skill in source) {
                if(skill.id == s ) {
                    return true;
                }
            }
            return false;
        }
    }
}
