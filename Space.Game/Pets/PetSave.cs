using System.Collections;
using System.Collections.Generic;

namespace Nebula.Pets {
    public class PetSave {
        public string id { get; set; }
        public int exp { get; set; }
        public int color { get; set; }
        public string type { get; set; }
        public int passiveSkill { get; set; }
        public List<Hashtable> activeSkills { get; set; }
        public bool active { get; set; }

        public float attackBaseAdd { get; set; }
        public float attackColorAdd { get; set; }
        public float attackLevelAdd { get; set; }

        public float hpBaseAdd { get; set; }
        public float hpColorAdd { get; set; }
        public float hpLevelAdd { get; set; }

        public float odBaseAdd { get; set; }
        public float odColorAdd { get; set; }
        public float odLevelAdd { get; set; }

        public int damageType { get; set; }
        public float killedTime { get; set; }

        public int mastery { get; set; }
    }
}
