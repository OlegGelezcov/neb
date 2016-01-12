using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003F4 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) {
                return false;
            }
            float hpPc = skill.data.Inputs.Value<float>("hp_pc");
            float hpTime = skill.data.Inputs.Value<float>("hp_time");
            var sourceDamagable = source.GetComponent<DamagableObject>();
            var hpVal = sourceDamagable.maximumHealth * hpPc;
            bool mastery = RollMastery(source);
            if(mastery) {
                hpTime *= 2;
            }
            sourceDamagable.SetRestoreHPPerSec(hpVal, hpTime, source.Id + skill.data.Id.ToString());
            return true;
        }
    }
}
