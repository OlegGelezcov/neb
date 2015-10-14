using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Bonuses;

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
            damagable.SetRestoreHPPerSec(hpForSec, hpTime);

            Buff buff = new Buff(skill.id, null, Common.BonusType.increase_healing_speed_on_pc, hpBuffTime, hpBuffPc);
            source.Bonuses().SetBuff(buff);
            return true;
        }
    }
}
