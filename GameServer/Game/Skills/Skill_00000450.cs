using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000450 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            var damagable = source.Damagable();

            float hpPc = skill.GetFloatInput("hp_pc");
            float hpTime = skill.GetFloatInput("hp_time");
            float hpBuffPc = skill.GetFloatInput("hpbuff_pc");
            float hpBuffTime = skill.GetFloatInput("hpbuff_time");

            float hpForSec = damagable.maximumHealth * hpPc / hpTime;
            string id = source.Id + skill.data.Id.ToString();

            bool mastery = RollMastery(source);
            if(mastery) {
                hpForSec *= 2;
                hpBuffTime *= 2;
            }

            damagable.SetRestoreHPPerSec(hpForSec, hpTime, id);

            

            Buff buff = new Buff(skill.id, null, Common.BonusType.increase_healing_speed_on_pc, hpBuffTime, hpBuffPc);
            source.Bonuses().SetBuff(buff, source);
            return true;
        }
    }
}
