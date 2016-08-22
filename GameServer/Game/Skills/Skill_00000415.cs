// Skill_00000415.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 12:46:43 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    //heal self when healing others
    public class Skill_00000415 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(Common.SkillUseState.normal);

            bool castOnTarget = true;
            if(source.Target().hasTarget ) {
                if(FriendTargetInvalid(source)) {
                    info.SetSkillUseState(Common.SkillUseState.invalidTarget);
                    castOnTarget = false;
                } else {
                    if(NotCheckDistance(source)) {
                        info.SetSkillUseState(Common.SkillUseState.tooFar);
                        castOnTarget = false;
                    }
                }
            } else {
                castOnTarget = false;
            }

            float hpPc = skill.GetFloatInput("hp_pc");
            float hpTime = skill.GetFloatInput("hp_time");

            bool mastery = RollMastery(source);
            if(mastery) {
                hpTime *= 2;
                hpPc *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }
            //source.Skills().Set415(hpTime, hpPc);

            if(castOnTarget) {
                source.Target().targetObject.Bonuses().SetBuff(new Buff(skill.data.Id.ToString() + source.Id, null, Common.BonusType.increase_healing_speed_on_pc, hpTime, hpPc), source);
            } else {
                source.Bonuses().SetBuff(new Buff(skill.data.Id.ToString() + source.Id, null, Common.BonusType.increase_healing_speed_on_pc, hpTime, hpPc), source);
            }

            return true;
        }
    }
}
