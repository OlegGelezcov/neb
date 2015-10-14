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
    public class Skill_0000041F : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) {
                return false;
            }

            float optimalDistancePc = skill.data.Inputs.Value<float>("optimal_distance_pc");
            float optimalDistanceTime = skill.data.Inputs.Value<float>("optimal_distance_time");

            if(CheckForShotFriend(source, skill)) {
                Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_optimal_distance_on_pc, optimalDistanceTime, optimalDistancePc);
                source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>().SetBuff(buff);
                return true;
            } else {
                Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_optimal_distance_on_pc, optimalDistanceTime, optimalDistancePc);
                source.GetComponent<PlayerBonuses>().SetBuff(buff);
                return true;
            }

        }
    }
}
