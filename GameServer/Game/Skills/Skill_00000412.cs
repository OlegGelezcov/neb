using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000412 : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            
            if(!source) {
                return false;
            }
            var damagable = source.GetComponent<DamagableObject>();
            if(!damagable) {
                return false;
            }

            string id = source.Id + skill.data.Id.ToString();
            float hpPc = skill.data.Inputs.Value<float>("hp_pc");
            float hpTime = skill.data.Inputs.Value<float>("hp_time");
            float hpVal = hpPc * damagable.maximumHealth;
            bool mastery = RollMastery(source);
            if(mastery) {
                hpTime *= 2;
            }
            damagable.SetRestoreHPPerSec(hpVal, hpTime, id);
            return true;
        }
    }
}
