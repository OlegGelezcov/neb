using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Bonuses;

namespace Nebula.Game.Skills
{
    public class Skill_00000438 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float cdPc = skill.GetFloatInput("cd_pc");
            float cdTime = skill.GetFloatInput("cd_time");
            float speedPc = skill.GetFloatInput("speed_pc");
            float speedTime = skill.GetFloatInput("speed_time");

            bool mastery = RollMastery(source);
            if(mastery) {
                cdTime *= 2;
                speedTime *= 2;
            }

            Buff cdBuff = new Buff(skill.id, null, Common.BonusType.decrease_cooldown_on_pc, cdTime, cdPc);
            Buff speedBuff = new Buff(skill.id, null, Common.BonusType.increase_speed_on_pc, speedTime, speedPc);
            var sourceBonuses = source.Bonuses();
            sourceBonuses.SetBuff(cdBuff, source);
            sourceBonuses.SetBuff(speedBuff, source);
            return true;
        }
    }
}
