using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Nebula.Game.Bonuses;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Game.Skills {
    public class Skill_0000041F : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(SkillUseState.normal);

            bool castOnTarget = true;
            if (source.Target().hasTarget) {
                if (FriendTargetInvalid(source)) {
                    castOnTarget = false;
                    
                } else {
                    if (NotCheckDistance(source)) {
                        info.SetSkillUseState(Common.SkillUseState.tooFar);
                        return false;
                    }
                }
            } else {
                castOnTarget = false;
            }


            float optimalDistancePc = skill.data.Inputs.Value<float>("optimal_distance_pc");
            float optimalDistanceTime = skill.data.Inputs.Value<float>("optimal_distance_time");

            bool mastery = RollMastery(source);
            if(mastery) {
                optimalDistanceTime *= 2;
                optimalDistancePc *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            if(castOnTarget) {
                Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_optimal_distance_on_pc, optimalDistanceTime, optimalDistancePc);
                source.Target().targetObject.Bonuses().SetBuff(buff, source);
                info.Add((int)SPC.Target, source.Target().targetObject.Id);
                info.Add((int)SPC.TargetType, source.Target().targetObject.Type);
            } else {
                Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_optimal_distance_on_pc, optimalDistanceTime, optimalDistancePc);
                source.Bonuses().SetBuff(buff, source);
                info.Add((int)SPC.Target, source.Id);
                info.Add((int)SPC.TargetType, source.Type);
            }
            return true;
        }
    }
}
