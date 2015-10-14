using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Common;
using System.Collections.Concurrent;

namespace Nebula.Resources {
    public class SkillDropping {
        private ConcurrentDictionary<Workshop, Dictionary<ShipModelSlotType, Dictionary<int, int[]>>> skills = new ConcurrentDictionary<Workshop, Dictionary<ShipModelSlotType, Dictionary<int, int[]>>>();


         
        public void Load(string inBasePath) {
            string[] files = { "Data/Skills/tank_skills.xml", "Data/Skills/heal_skills.xml", "Data/Skills/sdd_skills.xml", "Data/Skills/rdd_skills.xml" };
            foreach(var f in files) {
                string fullPath = Path.Combine(inBasePath, f);
                XDocument document = XDocument.Load(fullPath);
                Workshop[] workshops = document.Element("skill_dropping").GetString("workshops").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => {
                    return (Workshop)Enum.Parse(typeof(Workshop), s);
                }).ToArray();

                foreach(var w in workshops) {
                    var pair = LoadForWorkshop(w, document);
                    skills.TryAdd(pair.Key, pair.Value);
                }
            }
        }

        private KeyValuePair<Workshop, Dictionary<ShipModelSlotType, Dictionary<int, int[]>>> LoadForWorkshop(Workshop workshop, XDocument document) {
            var skillsDICT = document.Element("skill_dropping").Elements("slot").Select(slotElement => {
                ShipModelSlotType slotType = (ShipModelSlotType)Enum.Parse(typeof(ShipModelSlotType), slotElement.GetString("slot_type"));
                Dictionary<int, int[]> levelSkills = slotElement.Elements("level").Select(levelElement => {
                    int level = levelElement.GetInt("value");
                    string[] asSkills = levelElement.GetString("skills").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    int[] aiSkills = new int[asSkills.Length];
                    for (int i = 0; i < asSkills.Length; i++) {
                        aiSkills[i] = int.Parse(asSkills[i], System.Globalization.NumberStyles.HexNumber);
                    }
                    return new { LEVEL = level, SKILLS = aiSkills };
                }).ToDictionary(obj => obj.LEVEL, obj => obj.SKILLS);
                return new { SLOTTYPE = slotType, LEVELSKILLS = levelSkills };
            }).ToDictionary(obj => obj.SLOTTYPE, obj => obj.LEVELSKILLS);
            return new KeyValuePair<Workshop, Dictionary<ShipModelSlotType, Dictionary<int, int[]>>>(workshop, skillsDICT);
        }

        public int[] AllowedSkills(Workshop workshop, ShipModelSlotType slotType, int level) {
            Dictionary<ShipModelSlotType, Dictionary<int, int[]>> filtered = null;
            if(skills.TryGetValue(workshop, out filtered)) {
                return AllowedSkills(filtered, slotType, level);
            }
            return new int[] { };
        }

        private int[] AllowedSkills(Dictionary<ShipModelSlotType, Dictionary<int, int[]>> mTypeLevelSkills, ShipModelSlotType slotType, int level ) {
            Dictionary<int, int[]> filteredSkills = null;
            if(mTypeLevelSkills.TryGetValue(slotType, out filteredSkills)) {
                List<int> skillIDs = new List<int>();
                foreach(var pair in filteredSkills) {
                    if(pair.Key <= level) {
                        skillIDs.AddRange(pair.Value);
                    }
                }
                return skillIDs.Distinct().ToArray();
            }
            return new int[] { };
        }
    }
}
