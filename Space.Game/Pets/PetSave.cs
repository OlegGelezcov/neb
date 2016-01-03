using System.Collections.Generic;

namespace Nebula.Pets {
    public class PetSave {
        public string id { get; set; }
        public int exp { get; set; }
        public int color { get; set; }
        public string type { get; set; }
        public int passiveSkill { get; set; }
        public List<int> activeSkills { get; set; }
        public bool active { get; set; }
    }
}
