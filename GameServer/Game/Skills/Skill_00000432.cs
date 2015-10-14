using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000432 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float hpPc = skill.GetFloatInput("hp_pc");
            float hpTime = skill.GetFloatInput("hp_time");
            source.Skills().Set432(hpTime, hpPc);
            return true;
        }
    }
}
