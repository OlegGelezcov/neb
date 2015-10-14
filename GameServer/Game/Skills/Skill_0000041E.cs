using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Nebula.Game.Bonuses;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000041E : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) {
                return false;
            }

            float speedPc = skill.data.Inputs.Value<float>("speed_pc");
            float speedTime = skill.data.Inputs.Value<float>("speed_time");

            Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_speed_on_pc, speedTime, speedPc);
            source.GetComponent<PlayerBonuses>().SetBuff(buff);
            return true;
        }
    }
}
