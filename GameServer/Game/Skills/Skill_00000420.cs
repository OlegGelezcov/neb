// Skill_00000420.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 6:59:57 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    //speed debuff on target on 50%
    public class Skill_00000420 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(Common.SkillUseState.normal);
            if(ShotToEnemyRestricted(source, skill)) {
                info.SetSkillUseState(Common.SkillUseState.invalidTarget);
                return false;
            }
            if (NotCheckDistance(source)) {
                info.SetSkillUseState(Common.SkillUseState.tooFar);
                return false;
            }

            float speedPc = skill.GetFloatInput("speed_pc");
            float speedTime = skill.GetFloatInput("speed_time");

            var target = source.Target().targetObject;
            var targetMovable = target.Movable();
            if(!targetMovable) {
                return false;
            }

            bool mastery = RollMastery(source);
            if(mastery) {
                speedTime *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            Buff buff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.decrease_speed_on_pc, speedTime, speedPc);
            target.Bonuses().SetBuff(buff, source);
            return true;
        }
    }
}
