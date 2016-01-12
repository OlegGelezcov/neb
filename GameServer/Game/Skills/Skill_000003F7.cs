using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003F7 : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float hpPc = skill.GetDataInput<float>("hp_pc", 0f);
            float hpTime = skill.GetDataInput<float>("hp_time", 0f);
            bool mastery = RollMastery(source);
            if(mastery) {
                hpTime *= 2;
            }
            source.Skills().Set3F7(hpTime, hpPc);
            return true;
        }
    }
}
