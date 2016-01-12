using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Bonuses;
using Common;

namespace Nebula.Game.Skills {
    public class Skill_0000045A : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) {
                return false;
            }
            float speedPc = skill.data.Inputs.Value<float>("speed_pc");
            float speedTime = skill.data.Inputs.Value<float>("speed_time");

            var sourceBonuses = source.GetComponent<PlayerBonuses>();

            bool mastery = RollMastery(source);
            if(mastery) {
                speedTime *= 2;
            }

            Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_speed_on_pc, speedTime, speedPc);
            sourceBonuses.SetBuff(buff);
            return true;
        }
    }
}
