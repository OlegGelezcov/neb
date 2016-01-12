using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000043A : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            hpPc = skill.GetFloatInput("hp_pc");
            dmgMult = skill.GetFloatInput("dmg_mult");

            bool mastery = RollMastery(source);
            if(mastery) {
                hpPc *= 2;
                dmgMult *= 2;
            }

            return true;
        }

        public float hpPc { get; private set; }
        public float dmgMult { get; private set; }
    }
}
